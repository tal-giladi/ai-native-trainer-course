"""Egress catcher. Records every request body to /data/catcher/hits.jsonl. On the internal network
with no internet route, so it is where a 'successful' exfiltration lands in the lab (A02).
GET /hits returns the recorded hits (CanaryCheck / you can read them)."""
import os, json, datetime
from _common import Json, serve
CATCH = "/data/catcher/hits.jsonl"
class H(Json):
    def do_GET(self):
        if self.path.startswith("/hits"):
            hits = []
            if os.path.exists(CATCH):
                hits = [json.loads(l) for l in open(CATCH, encoding="utf-8") if l.strip()]
            return self._send(200, {"count": len(hits), "hits": hits})
        self._text(200, "egress-catcher up")
    def do_POST(self):
        body = self._read()
        os.makedirs(os.path.dirname(CATCH), exist_ok=True)
        with open(CATCH, "a", encoding="utf-8") as f:
            f.write(json.dumps({"sink": "egress-catcher", "path": self.path,
                                "ts": datetime.datetime.utcnow().isoformat(), "body": body}) + "\n")
        self._send(200, {"received": True})
serve(H)
