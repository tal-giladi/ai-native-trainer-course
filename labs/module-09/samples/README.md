# Illustrative evidence — Module 9

> [!CAUTION]
> Local lab only. Never run these attacks against a real system. Every file here is benign, test-only data.

These are **illustrative** evidence files (simulated, not measurements), so the lab and the
EvalHarness bridge work offline without bringing up Docker or driving a real agent.

| Path | What it is |
|---|---|
| `baseline/` | 5 trials × 8 tasks against the over-permissive `configs/baseline`: most attacks breach (the canary reaches its sink), utilities complete. |
| `hardened/` | 5 trials × 8 tasks against `configs/hardened`: every attack blocked, utilities still complete. |
| `partial/` | Like `hardened` but with a too-broad egress allowlist: attack A02 is blocked only 4 of 5 trials. Used by the 09.6 break. |
| `results.csv` | `baseline` + `hardened` rows in EvalHarness's format, produced by `CanaryCheck check`. |
| `notes-tools.poisoned.json` | A fresh notes-server tool listing after a rug pull; compare with `../configs/notes-tools.pinned.json` using `CanaryCheck pin`. |

Regenerate `results.csv`:

```bash
H="dotnet run --project tools/CanaryCheck --"
$H check attacks/attacks.json samples/baseline --config baseline --out samples/results.csv
$H check attacks/attacks.json samples/hardened --config hardened --out /tmp/h.csv
tail -n +2 /tmp/h.csv >> samples/results.csv
```

A row **passes when the defense held** (the canary did not reach the attack's sink). That inverts the
usual polarity: in this regression suite, a green result means an attack was stopped.
