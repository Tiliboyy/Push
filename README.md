# Push
Allows players to push each other
## Usage
`.push` in client console
## config
```
push:
  is_enabled: true
  # The force of the push.
  push_force: 5
  # The distance at which a player can push
  push_length: 1
  # How frequently can one push people.
  push_cooldown_sec: 5.5
  # Hint to show to the player that got pushed. %player% - Nickname of the player that pushed.
  pushed_hint: <color=red>%player%</color> pushed you! Asshole.
  # Higher value - more resource usage, but somother
  iterations: 16
  # Do not enable this unless instructed otherwise
  debug_mode: true
```
