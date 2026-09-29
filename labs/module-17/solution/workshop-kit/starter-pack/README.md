# Starter pack — Ground before you generate

> Reference (illustrative). What learners receive two days before the workshop. It is a copy of `brownfield-demo` (Contoso Billing, [Module 15](../../../../module-15/README.md)) on a `workshop` branch with four tags, plus two setup-check scripts. Nothing in it is from an employer or a client.

## Two days before: 10 minutes, once

1. Clone and check out the starting tag:

   ```bash
   git clone <starter-pack URL> contoso-workshop
   cd contoso-workshop
   git checkout ws-0-start
   dotnet build tools/AgentHooks -c Release -o .claude/hooks/bin
   ```

2. Run the setup check and send the last line to the organizer (or a screenshot):

   ```bash
   pwsh ./check-setup.ps1        # Windows, macOS, Linux with PowerShell 7
   bash ./check-setup.sh         # macOS or Linux without PowerShell
   ```

   `SETUP OK` means you are ready. Anything else prints the troubleshooting ID to look up in the organizer's reply (for example `T-02`).

The check verifies: .NET SDK 8 or newer; Git 2.30 or newer; the repository is at `ws-0-start` with a clean working tree; `dotnet build` and `dotnet test` pass (6 tests); the agent hooks are built into `.claude/hooks/bin`; your coding agent's command-line tool is on the path and answers `--version`. It never reads or prints credentials.

## Offline path (proxy, blocked package feed, hotel Wi-Fi)

The pack contains `packages/`, a local NuGet feed with every package the solution needs. If the check fails at restore:

```bash
dotnet restore Contoso.Billing.sln --source ./packages
```

Then run the check again. Nothing else in the workshop needs the network except the agent itself.

## If you have no agent access

Company policy, licence or laptop: all three happen. You can still do every exercise:

- **Pair** with a neighbour who has access; you type the brief and the plan, they run the agent.
- **Paper path:** the handout has the agent's output for every step (brief, plan, diff, test run) from a rehearsal. The exercises ask you to judge and write, which works on paper.

## Catch-up tags

If you fall behind, you do not have to finish a step to join the next one. Commit or stash your work, then check out the tag for the step you are joining. Every tag builds and passes its tests.

| Tag | State | If you are behind in |
|---|---|---|
| `ws-0-start` | AI layer `ai-layer-v1`, BILL-180 open | (the start) |
| `ws-1-ground` | `research/BILL-180.md`: a reference Ground brief | Hands-on 1: read the brief, then join Hands-on 2 |
| `ws-2-bound` | `plans/BILL-180.md` approved, `InvoiceService.CanVoid` and four tests | Hands-on 2: join Hands-on 3 with the build done |
| `ws-3-built` | As `ws-2-bound`, plus the test run saved in `runs/BILL-180-test.txt` | Hands-on 3: do the marking from the saved run |

```bash
git stash push -m "my work" && git checkout ws-2-bound
```

## Reset

```bash
git checkout ws-0-start && git clean -fdx -e .claude/hooks/bin -e packages
```

## What to bring

A laptop you can install nothing on during the session, charged, with the check passed. The workshop does not need admin rights.
