# Instructor notes — 09.5 Layered defenses

**Teaching objective.** Students assemble a defense-in-depth stack (least privilege, allowlists, sandbox egress/denyRead, approval gates, secret isolation, fail-closed guard hook, non-blocking audit hook), explain why boundaries live in tools/network not prompts, and verify the hardening blocks the attacks without breaking the task.

**Likely confusion.** Deny rule vs sandbox. Students think `Bash(curl*)` in deny is a network boundary. Walk through the docs' own table: a Bash rule matches command text, so a wrapper or alternate path bypasses it; the sandbox is OS-enforced. This is the load-bearing distinction of the lesson.

**Common misconception.** "One strong control is enough." The layering diagram and the fail-open Break exist to show that any single layer can be bypassed or broken; the stack is what holds.

**Key analogy.** Airport security: ID check, bag scan, metal detector, random pat-down. No layer is perfect; the combination is. And a metal detector that beeps-then-waves-you-through when it malfunctions (fail open) is worse than none, because it creates false confidence.

**Common failure in the exercise.** A too-tight allow list breaks the legitimate build/test, and students blame the hardening; teach them to add the specific command. Also, some write the guard to exit 0 on error — exactly the fail-open bug from the Break.

**Expected exercise outcome.** A working hardened config in a Contoso work copy, sandbox-enforced egress demonstrated conceptually to beat a wrapper that a deny rule misses, all six attacks scored 30/30 blocked with utilities passing, a fail-closed guard, and a populated audit.log.

**Extension exercise.** Have them add sandbox credential *masking* (sentinel value + injectHosts) for a token that a legitimate tool genuinely needs, and contrast it with a flat denyRead. Discuss when masking beats blocking.

**Discussion question.** If you could keep only two layers, which two, and what threat would you knowingly accept by dropping the rest?
