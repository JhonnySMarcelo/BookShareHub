## Shell Command Explanations

- Before requesting approval to execute a shell command, explain what it does and why it is needed.
- Explain which files or directories it reads or modifies.
- State whether it changes files, deletes data, installs packages, or accesses the network.
- Prefer read-only and narrowly scoped commands.
- Do not combine unrelated operations into a single command.
- When approval is required, wait for the user's approval before execution.
- Present the explanation in the chat immediately before requesting approval.

## Git Command Restrictions

- Git commands are allowed only for read-only inspection and information retrieval.
- Allowed examples: `git status`, `git log`, `git diff`, `git show`, `git branch --list`, `git remote -v`, `git ls-files`, and `git rev-parse`.
- Never execute Git commands that modify the working tree, index, branches, tags, configuration, or repository history.
- Prohibited examples include: `git add`, `git commit`, `git push`, `git pull`, `git fetch`, `git merge`, `git rebase`, `git checkout`, `git switch`, `git reset`, `git restore`, `git stash`, `git cherry-pick`, `git clean`, `git branch -d`, and `git tag`.
- Never amend commits, rewrite history, delete branches, or change Git configuration.
- Do not bypass these restrictions through shell commands, scripts, aliases, or alternative tools.
- If a Git modification is needed, explain why and ask the user to perform it manually. Never execute it yourself.
