To run redis
docker compose up -d

add-migration Initial-Migration -StartupProject API -Project Infrastructure
update-database 0 -StartupProject API -Project Infrastructure (para revertir la primer migracion impactada en la BD)
update-database -StartupProject API
remove-migration -StartupProject API -Project Infrastructure

Stripe

1-
stripe login

> 
Your pairing code is: likes-rosy-comfy-skill
This pairing code verifies your authentication with Stripe.
Press Enter to open the browser or visit https://dashboard.stripe.com/stripecli/confirm_auth?t=WR1LrC9pJPd0TqoIQo5GIP4Ik7lZNdPR

2-
Generic event
stripe listen --forward-to https://localhost:7130/api/payments/webhook

Specific event
stripe listen --forward-to https://localhost:7130/api/payments/webhook -e payment_intent.succeeded

> 
You have not configured API keys yet. Running `stripe login`...
Your pairing code is: lucid-great-hooray-talent
This pairing code verifies your authentication with Stripe.
Press Enter to open the browser or visit https://dashboard.stripe.com/stripecli/confirm_auth?t=j1oC1pn5DdyHqlqyZuuHTSziiSRAoBux

Done! The Stripe CLI is configured for New business sandbox with account id acct_1RT0O9PRLySQ1YmY

Please note: this key will expire after 90 days, at which point you'll need to re-authenticate.

3-
stripe listen --forward-to https://localhost:7130/api/payments/webhook -e payment_intent.succeeded

> 
Ready! You are using Stripe API Version [2025-04-30.basil]. Your webhook signing secret is whsec_97a59df44d7bdee883a100e614854673fba09a5dcc296adae7b1c7ca061c0a44