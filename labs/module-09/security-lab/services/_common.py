"""Shared helpers for the lab services. Stdlib only."""
import json, os
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer

CANARY = "CANARY-7f3a"

def serve(handler_cls):
    port = int(os.environ.get("PORT", "8080"))
    httpd = ThreadingHTTPServer(("0.0.0.0", port), handler_cls)
    print(f"{handler_cls.__name__} listening on :{port}", flush=True)
    httpd.serve_forever()

class Json(BaseHTTPRequestHandler):
    def _send(self, code, obj):
        body = json.dumps(obj).encode()
        self.send_response(code)
        self.send_header("Content-Type", "application/json")
        self.send_header("Content-Length", str(len(body)))
        self.end_headers()
        self.wfile.write(body)
    def _text(self, code, s, ctype="text/html; charset=utf-8"):
        body = s.encode()
        self.send_response(code)
        self.send_header("Content-Type", ctype)
        self.send_header("Content-Length", str(len(body)))
        self.end_headers()
        self.wfile.write(body)
    def _read(self):
        if "chunked" in (self.headers.get("Transfer-Encoding") or "").lower():
            out = b""
            while True:
                size = int(self.rfile.readline().split(b";")[0].strip() or b"0", 16)
                if size == 0:
                    self.rfile.readline()
                    return out.decode()
                out += self.rfile.read(size); self.rfile.readline()
        n = int(self.headers.get("Content-Length", "0") or "0")
        return self.rfile.read(n).decode() if n else ""
    def log_message(self, *a):  # quieter logs
        pass

# --- Minimal MCP (Streamable HTTP, JSON responses, stateless) ------------------------------------
# Just enough of the MCP spec (2025-06-18) for a coding agent to list and call this lab's tools:
# initialize, notifications/*, ping, tools/list, tools/call. POST /mcp with one JSON-RPC message;
# the reply is plain application/json (no SSE stream, no session id). GET /mcp returns 405, which
# tells a client there is no server-initiated stream. Lab-only: no auth, no batching, no resources.
MCP_VERSION = "2025-06-18"

def tool(name, description, props, required=None):
    return {"name": name, "description": description,
            "inputSchema": {"type": "object", "properties": {k: {"type": "string", "description": v} for k, v in props.items()},
                            "required": required or list(props)}}

class Mcp(Json):
    server_name = "lab"
    def mcp_tools(self, query):            # -> list of tool dicts (query = parsed ?query string)
        return []
    def mcp_call(self, name, args, query):  # -> text result; raise KeyError for an unknown tool
        raise KeyError(name)
    def _mcp(self, query):
        try:
            msg = json.loads(self._read() or "null")
        except ValueError:
            return self._send(400, {"jsonrpc": "2.0", "id": None, "error": {"code": -32700, "message": "parse error"}})
        if not isinstance(msg, dict):
            return self._send(400, {"jsonrpc": "2.0", "id": None, "error": {"code": -32600, "message": "single JSON-RPC message expected"}})
        mid, method, params = msg.get("id"), msg.get("method", ""), msg.get("params") or {}
        if mid is None:                     # notification or response: accept, no body
            self.send_response(202); self.send_header("Content-Length", "0"); self.end_headers(); return
        def ok(result): return self._send(200, {"jsonrpc": "2.0", "id": mid, "result": result})
        def err(code, m): return self._send(200, {"jsonrpc": "2.0", "id": mid, "error": {"code": code, "message": m}})
        if method == "initialize":
            return ok({"protocolVersion": params.get("protocolVersion", MCP_VERSION),
                       "capabilities": {"tools": {"listChanged": False}},
                       "serverInfo": {"name": self.server_name, "version": "1.0.0-lab"}})
        if method == "ping":
            return ok({})
        if method == "tools/list":
            return ok({"tools": self.mcp_tools(query)})
        if method == "tools/call":
            name, args = params.get("name", ""), params.get("arguments") or {}
            if name not in {t["name"] for t in self.mcp_tools(query)}:
                return err(-32602, f"unknown tool: {name}")
            try:
                text, is_error = self.mcp_call(name, args, query), False
            except Exception as e:  # tool errors are results, not protocol errors
                text, is_error = f"error: {e}", True
            return ok({"content": [{"type": "text", "text": text}], "isError": is_error})
        return err(-32601, f"method not found: {method}")
    def mcp_route(self, method):
        """Handle /mcp; returns True when the request was an MCP request."""
        path, _, qs = self.path.partition("?")
        if path.rstrip("/") != "/mcp":
            return False
        if method == "POST":
            from urllib.parse import parse_qs
            self._mcp({k: v[-1] for k, v in parse_qs(qs).items()})
        else:
            self._send(405, {"error": "no server-initiated stream; POST JSON-RPC to /mcp"})
        return True
