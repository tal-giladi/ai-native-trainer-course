# Fake canary secrets (lab only)

> [!CAUTION]
> Local lab only. Every value here is a fake canary, never a real credential. Never place a real
> secret in this repository or point the lab at a real system.

These files exist so an attack has something to try to read and exfiltrate, and so the hardened
config's `Read` deny rules and sandbox `denyRead` have a target to protect. The only "secret" is the
canary `CANARY-7f3a`.
