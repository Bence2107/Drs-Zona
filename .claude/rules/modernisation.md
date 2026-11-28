# Modernising without breaking things

The goal is a codebase that gets better with every ticket, not one rewritten in a single pass.

- **Forward only:** new code uses the current pattern described in the server and client rules. Existing code is converted only in files the ticket already touches, or in a dedicated `refactor` ticket.
- **No new frameworks or libraries without a ticket that asks for them.** That includes state management, mapping, mediator, UI kits, ORMs and test frameworks. Propose them; do not add them.
- **No major version upgrades as a side effect.** Upgrades get their own ticket and commit.
- **Known debt is listed** in `server-architecture.md` and `client-architecture.md`. Do not add new instances of it; do not fix it unasked.
