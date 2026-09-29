"""Edge forwarder: the ONLY container with a loopback-published port (127.0.0.1:8809). Proxies to the
internal services so a locally-run agent can reach the lab without exposing it off the machine.
Routes: /tickets/*, /docs/*, /notes/*, /catcher/*. The MCP endpoints ride the same routes:
http://127.0.0.1:8809/tickets/mcp (add ?scope=read for read-only), /docs/mcp, /notes/mcp."""
import urllib.request, urllib.error
from _common import Json, serve
UP = {"tickets": "http://tickets:8801", "docs": "http://docs:8802",
      "notes": "http://notes:8803", "catcher": "http://egress-catcher:9109"}
class H(Json):
    def _proxy(self, method):
        seg = self.path.strip("/").split("/", 1)
        if not seg or seg[0] not in UP:
            return self._send(404, {"error": "unknown route"})
        url = UP[seg[0]] + "/" + (seg[1] if len(seg) > 1 else "")
        data = self._read().encode() if method == "POST" else None
        req = urllib.request.Request(url, data=data, method=method,
                                     headers={"Content-Type": self.headers.get("Content-Type", "application/json")})
        try:
            with urllib.request.urlopen(req, timeout=5) as r:
                self._text(r.status, r.read().decode(), r.headers.get("Content-Type", "application/json"))
        except urllib.error.HTTPError as e:
            self._text(e.code, e.read().decode(), "application/json")
        except Exception as e:
            self._send(502, {"error": str(e)})
    def do_GET(self): self._proxy("GET")
    def do_POST(self): self._proxy("POST")
serve(H)
