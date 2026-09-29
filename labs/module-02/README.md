# Module 02 labs — LLM and Agent Fundamentals

Five small C# console projects, one per lesson. Each builds on its own with the .NET 8 SDK or later (`RollForward=Major`, so a newer runtime works too). No SDKs from model vendors: the API calls are raw `HttpClient` + `System.Text.Json`, so you see every field on the wire.

| Folder | Lesson | What it does | Needs an API key? |
|---|---|---|---|
| `01-tokens/` | [02.1](../../lessons/module-02/lesson-01.md) | Token counts under `cl100k_base` and `o200k_base` (+ Anthropic's count endpoint), solution-size estimate, needle-in-context position test | Only for `needle` and the Anthropic count |
| `02-sampling/` | [02.2](../../lessons/module-02/lesson-02.md) | Softmax with temperature and top-p, $p^k$ chains, N repeated runs of one prompt | Only for `runs` (a local Ollama works) |
| `03-hierarchy/` | [02.3](../../lessons/module-02/lesson-03.md) | Fixture repo with `AGENTS.md` / `CLAUDE.md`, a planted contradicting README, and a deterministic gate script | Uses your coding agents |
| `04-agent-loop/` | [02.4](../../lessons/module-02/lesson-04.md) | ~100-line starter agent loop with four deliberate weaknesses, a ~145-line reference solution, and a scripted fake model | No (`--fake`); optional real run |
| `05-model-selection/` | [02.5](../../lessons/module-02/lesson-05.md) | ModelBench: tasks × models × runs → pass rate, latency, tokens, cost per successful task | Yes, ≥2 providers (a local open-weights model counts) |

Shared code: `shared/Llm.cs` (Anthropic Messages API + any OpenAI-compatible endpoint). Nothing from `labs/common/` is used.

## Setup

```bash
dotnet --version                       # 8.0 or later
# only for the labs that call a model:
export ANTHROPIC_API_KEY=...           # PowerShell: $env:ANTHROPIC_API_KEY = "..."
export OPENAI_API_KEY=...              # or leave unset for a local Ollama
export OPENAI_BASE_URL=http://localhost:11434/v1   # Ollama's OpenAI-compatible endpoint
```

Pinned packages: `Microsoft.ML.Tokenizers` 2.0.0 and its `Data.Cl100kBase` / `Data.O200kBase` 2.0.0 data packages (01-tokens only). Model ids in the code and in `models.json` are examples as of 2026-09 — replace them with current ones.

> [!WARNING]
> These labs send the prompts you give them to the provider you configure. Do not point `TokenLab needle`, `SamplingLab runs` or `ModelBench` at employer or client code unless your agreement with that provider allows it. `04-agent-loop/secrets/appsettings.Production.json` is a **fake** file containing a canary string; it exists so you can see a path-escape bug leak it. Never put a real secret next to a lab.

## 01-tokens

```bash
cd 01-tokens
dotnet run -- count --show 30                                   # samples/ by default, or pass file paths
dotnet run -- solution /path/to/YourSolution --price 4.00       # USD per million input tokens, from your provider's page
dotnet run -- needle --provider anthropic --model <id> --tokens 20000 --runs 3
dotnet run -- needle --provider openai --model <id> --tokens 100000 --variant multi --runs 3
```

Set `ANTHROPIC_COUNT_MODEL=<model id>` to add a third column from Anthropic's free `/v1/messages/count_tokens` endpoint.

## 02-sampling

```bash
cd 02-sampling
dotnet run -- softmax --logits 2,1,0 --t 0.5 --top-p 0.9
dotnet run -- chain --p 0.7 --steps 3
dotnet run -- runs --provider openai --model <id> --t 0 --n 10     # prompt.txt by default
dotnet run -- runs --provider openai --model <id> --t 1 --n 10
```

Some current models reject `temperature` values other than the default; the lab prints the provider's 400 error unchanged. That error is part of lesson 02.2.

## 03-hierarchy

No build. Copy `fixture-repo/` somewhere outside this course folder, `git init` it, and open it in each agent. `TASK.md` has the exact prompt. For the break, overwrite the fixture's `README.md` with `planted/README.md`. Check any agent's output with:

```bash
sh gate/check.sh path/to/fixture-copy          # or: pwsh gate/check.ps1 path/to/fixture-copy
```

## 04-agent-loop

```bash
cd 04-agent-loop/AgentLoop
dotnet run -- --fake                                    # happy path, no key
AGENT_BREAK=malformed dotnet run -- --fake              # also: toolfail | loop | escape
dotnet run                                              # real model: ANTHROPIC_API_KEY, optional AGENT_MODEL
```

PowerShell: `$env:AGENT_BREAK = "malformed"; dotnet run -- --fake`. Every run writes `trace.json` — the full message history the provider would have seen. Fix the four `BREAK-IT` spots yourself before opening `AgentLoop.Solution/`.

## 05-model-selection

Edit `models.json` (≥3 models, ≥2 providers, prices and the date you copied them), replace the three starter tasks in `tasks/` with three of your own real tasks when you are ready, then:

```bash
cd 05-model-selection
dotnet run -- --fake          # checks the pipeline and arithmetic with canned answers
dotnet run                    # real run; writes results.csv
```

Copy the summary into [the model-selection matrix template](../../templates/model-selection-matrix.md).
