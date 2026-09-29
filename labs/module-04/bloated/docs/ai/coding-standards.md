# Contoso C# Coding Standards (company-wide, all teams)

_This document applies to every .NET team at Contoso. Pasted in full so the agent follows it._

## 1. General principles

- Write code for humans first, computers second.
- Follow the SOLID principles.
- Follow DRY (Don't Repeat Yourself) and KISS (Keep It Simple, Stupid).
- Follow YAGNI (You Aren't Gonna Need It).
- Prefer composition over inheritance.
- Program to interfaces, not implementations.
- Write clean, maintainable, readable code.
- Leave the code better than you found it (the Boy Scout rule).
- Make it work, make it right, make it fast — in that order.
- Code is read far more often than it is written.

## 2. Naming

- Use PascalCase for classes, records, structs, enums, methods, properties and events.
- Use camelCase for local variables and parameters.
- Prefix private fields with an underscore: `_invoiceRepository`.
- Prefix interfaces with `I`: `IInvoiceRepository`.
- Do not use Hungarian notation.
- Do not use abbreviations unless they are well known (`Id`, `Url`, `Html`).
- Use meaningful names; avoid `data`, `info`, `temp`, `obj`.
- Boolean names should read as questions: `isOverdue`, `hasLines`.
- Async methods end with `Async`.
- Name test methods as `Method_Scenario_ExpectedResult` or as a readable sentence.
- Name constants in PascalCase, not SCREAMING_CASE.
- Name generic type parameters `T` or `TSomething`.

## 3. Layout and formatting

- Use 4 spaces for indentation, never tabs.
- Use Allman braces (opening brace on its own line).
- Always use braces, even for one-line `if` statements.
- One statement per line.
- One declaration per line.
- Maximum line length: 120 characters.
- One blank line between methods.
- No more than one consecutive blank line.
- Put `using` directives outside the namespace.
- Sort `using` directives alphabetically, `System` first.
- Use file-scoped namespaces in new files.
- Order members: fields, constructors, properties, methods.
- Order by accessibility: public, internal, protected, private.
- Remove trailing whitespace.
- End every file with a newline.

## 4. Language features

- Use `var` when the type is obvious from the right-hand side.
- Use explicit types when the type is not obvious.
- Use expression-bodied members for one-line properties and methods.
- Use pattern matching where it makes code clearer.
- Use `switch` expressions instead of long `if`/`else` chains.
- Use records for immutable data.
- Use `init` accessors for immutable properties.
- Use nullable reference types and fix all nullable warnings.
- Use `string.IsNullOrWhiteSpace` instead of comparing to empty strings.
- Use string interpolation instead of concatenation.
- Use `nameof` instead of string literals for member names.
- Use collection expressions where supported.
- Use primary constructors only for simple types.
- Use `readonly` wherever possible.
- Avoid `dynamic`.
- Avoid `goto`.
- Avoid regions.
- Avoid `#if` directives except for platform code.

## 5. Methods and classes

- Keep methods under 30 lines.
- Keep classes under 300 lines.
- A method should do one thing.
- A class should have one reason to change.
- Limit parameters to four; use a parameter object beyond that.
- Avoid output parameters.
- Avoid static classes except for extension methods and pure helpers.
- Seal classes that are not designed for inheritance.
- Make fields private.
- Validate arguments at public boundaries.
- Prefer immutability.

## 6. Asynchronous code

- Use `async`/`await` for all I/O.
- Never use `.Result` or `.Wait()`.
- Never use `async void` except for event handlers.
- Pass a `CancellationToken` through every async call chain.
- Use `ConfigureAwait(false)` in library code.
- Do not wrap synchronous code in `Task.Run` in library code.
- Use `ValueTask` only when you have measured a benefit.
- Avoid fire-and-forget tasks.

## 7. Exceptions

- Throw exceptions for exceptional situations only.
- Catch specific exception types.
- Never catch `Exception` without rethrowing, except at the top level.
- Use `throw;` to rethrow, not `throw ex;`.
- Include useful messages in exceptions.
- Create custom exceptions only when callers need to handle them differently.
- Use guard clauses (`ArgumentNullException.ThrowIfNull`).

## 8. Collections and LINQ

- Return empty collections, never `null`.
- Expose `IReadOnlyList<T>` or `IReadOnlyCollection<T>` from public APIs.
- Use LINQ for readability, loops for hot paths.
- Avoid multiple enumeration of `IEnumerable<T>`.
- Use `Any()` instead of `Count() > 0`.
- Use `FirstOrDefault` carefully; handle the default.

## 9. Dependency injection

- Use constructor injection.
- Do not use the service locator pattern.
- Register services in the composition root only.
- Prefer scoped lifetimes for services that touch the database.
- Do not inject `IServiceProvider` into business classes.
- The current time is injected: use `IClock.UtcNow`, never read the clock directly in services.

## 10. Logging

- Use `ILogger<T>` from `Microsoft.Extensions.Logging`.
- Use message templates, not string interpolation, in log calls.
- Use the right log level.
- Never log secrets or personal data.
- Use `LoggerMessage` source generators in hot paths.

## 11. Testing

- Use xUnit for unit tests.
- Use FluentAssertions for readable assertions (optional).
- Use NSubstitute or Moq for mocking.
- Follow Arrange-Act-Assert.
- One logical assertion per test.
- Tests must not depend on each other.
- Tests must not depend on the current time; inject a clock.
- Tests must not depend on the file system or the network (unit tests).
- Name tests so that a failure explains itself.
- Do not test private methods directly.
- Test behaviour, not implementation.
- Keep tests fast (under 100 ms each).

## 12. Documentation

- Add XML documentation comments to all public types and members.
- Explain *why*, not *what*, in comments.
- Keep comments up to date; delete comments that lie.
- Do not leave commented-out code.
- Use `TODO` comments only with a ticket number.
- Keep a README in every repository.

## 13. Security

- Never commit secrets; use the secret store.
- Validate all input from outside the process.
- Use parameterized queries.
- Encode output for its context (HTML, URL, SQL).
- Use the latest supported TLS version.
- Follow the principle of least privilege.
- Keep dependencies up to date; fix critical vulnerabilities within 7 days.
- Review the OWASP Top 10 once a year.

## 14. Performance

- Measure before optimizing.
- Avoid premature optimization.
- Use `StringBuilder` in loops.
- Avoid allocations in hot paths.
- Use `Span<T>` where it helps and is readable.
- Cache expensive results with a clear invalidation strategy.
- Avoid N+1 queries.
- Page large result sets.

## 15. Front-end (for full-stack developers)

- Use TypeScript in strict mode.
- Use ESLint and Prettier with the shared configuration.
- Use React functional components and hooks.
- Keep components small and focused.
- Use `npm ci` in CI pipelines.
- Write unit tests with Jest and React Testing Library.

## 16. Python scripts

- Use Python 3.11 or newer.
- Format with black; lint with ruff.
- Pin dependencies in `requirements.txt`.
- Use type hints.
- Test scripts with pytest.

## 17. Code review checklist

- Does the code do what the ticket asks?
- Is it readable?
- Is it tested?
- Is it secure?
- Is it fast enough?
- Does it follow these standards?
- Is the documentation updated?
- Would you be happy to maintain it?
