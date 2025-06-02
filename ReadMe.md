User
Tom@test.com
Pa$$w0rd

To run redis locally
docker compose up -d

Entity Framework

add-migration Initial-Migration -StartupProject API -Project Infrastructure
update-database 0 -StartupProject API -Project Infrastructure (para revertir la primer migracion impactada en la BD)
update-database -StartupProject API
remove-migration -StartupProject API -Project Infrastructure

Test Stripe locally

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
Run this command to test strpie locally
stripe listen --forward-to https://localhost:7130/api/payments/webhook -e payment_intent.succeeded

> 
Ready! You are using Stripe API Version [2025-04-30.basil]. Your webhook signing secret is whsec_97a59df44d7bdee883a100e614854673fba09a5dcc296adae7b1c7ca061c0a44

Upstash

"Redis": "host:port, password=pw,ssl=true,abortConnect=False"
"Redis": "just-asp-27676.upstash.io:6379,password=AWwcAAIjcDFlMDAzYTRhYjUwN2I0NDJiYjM1NTI1YWJmNWJkZWQ3MnAxMA,ssl=True,abortConnect=False"
"Redis": "SkinetTest.redis.cache.windows.net:6380,password=nZCeSltOMQGqR87fO8pANdfSSbWIVqD3NAzCaF38id4=,ssl=True,abortConnect=False"

"Redis": "just-asp-27676.upstash.io:6379,password=AWwcAAIjcDFlMDAzYTRhYjUwN2I0NDJiYjM1NTI1YWJmNWJkZWQ3MnAxMA,ssl=True,abortConnect=False"

Connect to your database (Redis)
redis-cli --tls -u redis://default:AWwcAAIjcDFlMDAzYTRhYjUwN2I0NDJiYjM1NTI1YWJmNWJkZWQ3MnAxMA@just-asp-27676.upstash.io:6379

Azure

"Redis": "redis-skinet.redis.cache.windows.net:6380,password=jOvRE0sodqSUYu6i01QkvgO3XWYm6xkgjAzCaM4WF8s=,ssl=True,abortConnect=False"

Azure SQL Server

Database name: Skinet

Server name: skinet-server-name
Server admin login: appuser
Pass: Pa$$w0rd

Stripe

Your webhook signing secret for deployed app is: whsec_F0NmeyVCLLlqSACKALoNPIfhbXBddVgh