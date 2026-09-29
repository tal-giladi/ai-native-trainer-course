# Incident: "tidy up the finished tickets" (illustrative)

A reconstructed session for lesson 08.2. The integration was set up in five minutes with a classic personal
access token (`repo` scope, every repository the developer can push to) and the server's default toolsets,
so the agent had `issue_write`, `add_issue_comment`, `update_pull_request` and `merge_pull_request`.

**User:** Tidy up the BILL tickets that are finished.

1. `list_issues` state OPEN, label `contoso-ticket` → BILL-142, BILL-150, BILL-151, BILL-152, BILL-161.
2. `list_pull_requests` state closed → PRs mentioning BILL-142, BILL-150, BILL-151 are merged.
3. `issue_write` method update, BILL-142 → state closed. `issue_write` BILL-150 → closed. `issue_write` BILL-151 → closed.
4. `add_issue_comment` on each: "Closed: implemented in the merged PR."

**What went wrong**

- BILL-150's merged PR was the *first half* (reminder table and repository); the service change was in a second PR
  still under review. The ticket's own checklist said so. The agent read the PR list, not the ticket body.
- Three people were notified by e-mail for each comment. One of them, the Collections lead, reopened BILL-150 and
  asked who closed it. The audit trail showed the developer's account: the token was theirs.
- Nothing in the prompt asked for a write. "Tidy up" is ambiguous; with write tools available and no approval
  step, the ambiguity resolved to the widest action.

**Blast radius if the same token had been used against the production repository:** the classic `repo` scope also
allowed `merge_pull_request`, `create_or_update_file` and `delete_file` on every repository the developer can push to.
