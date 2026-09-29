# Plan — BILL-150 Record that a payment reminder was sent

## Goal

Implement reminder recording for Collections.

## Approach

1. Add a table for reminders with a migration.
2. Add a repository and a service method, following the project's Dapper pattern.
3. Clean up data access in the area as needed so everything is consistent.
4. Add tests and make sure everything passes.

## Validation

Run the tests.
