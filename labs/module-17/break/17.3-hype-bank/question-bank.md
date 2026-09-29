# Hard-questions bank (first draft)

> Break for lesson 17.3. Written the evening before a first paid workshop, from memory, in the voice of the presenter's favourite conference talks. Run `KitCheck questions` on it, then read each answer aloud with a timer.

### Q01 · "Is AI going to replace us?"
- Category: replace
- Answer: Absolutely not, and anyone who tells you otherwise is selling something. AI is a tool, like the compiler or the IDE. Developers who use AI will replace developers who don't, that's the real story. Everyone I've worked with who adopted this method became more valuable, not less. So there's really nothing to worry about as long as you keep learning.
- Evidence:
- Bridge: "Let's get back to the demo."

### Q02 · "What stops a ticket from telling the agent to do something bad?"
- Category: security
- Answer: The models are trained to refuse harmful instructions, and Claude Code asks for permission before it does anything dangerous, so in practice it's completely safe. I've never seen it happen in two years of daily use. Security teams worry about this far too much, honestly. The real risk is not using AI and falling behind your competitors, which is a much bigger threat to your business than any prompt injection could ever be.
- Evidence: my experience

### Q03 · "What's the ROI?"
- Category: roi
- Answer: Teams that adopt this method see 10x productivity on routine tickets and around 55% faster delivery overall. In my own team cycle time dropped 16%, and that was before we had the full method in place, so you can expect more. Most teams pay back the cost of this workshop within the first two weeks. It's guaranteed to pay for itself.
- Evidence: EXP-01
- Not known: -

### Q04 · "Isn't this just DRY?"
- Category: skeptic
- Answer: Great question! So, let me tell you about the time we found the balance calculated twice. It was a Tuesday, and Finance called because an invoice showed two different numbers, and we spent the whole afternoon tracing it. First we looked at the C# code and the tests were all green, and then someone had the idea to check the stored procedures, and it turned out the agent had written its own calculation in C# right next to usp_GetCustomerBalance, which already did the same thing but with a slightly different rule for credit notes. And that's when I realized that this isn't DRY at all, it's something new that only happens with agents, because a human would never do that. So we built the whole method around that one insight, and since then we've never had a single shadow rule. So no, it's definitely not just DRY, it's a completely different problem that needs a completely different solution, which is exactly what this workshop is about.
- Evidence: INC-08
- Not known: none

### Q05 · "Does our code go to the vendor?"
- Category: tools
- Answer: Let's take that offline, it depends on a lot of things.
- Evidence: -
