# Skinet E-Commerce API

A modern e-commerce API built with .NET 9, Entity Framework Core, Redis, and Stripe integration.

---

## Table of Contents
- [Features](#features)
- [Getting Started](#getting-started)
- [Development](#development)
- [Database & Migrations](#database--migrations)
- [Redis Setup](#redis-setup)
- [Stripe Integration](#stripe-integration)
- [Sample Users](#sample-users)
- [Azure Deployment](#azure-deployment)

---

## Features
- Product catalog, orders, and payments
- User authentication (sample users provided)
- Redis caching
- Stripe payment processing

---

## Getting Started

1. **Clone the repository**
2. **To run redis locally**
- docker compose up -d
3. **Restore dependencies**
- dotnet restore
4. **Run the API**
- dotnet run --project API
---

## Development

### Entity Framework Core
- Add migration: add-migration <MigrationName> -StartupProject API -Project Infrastructure
- Update database: update-database -StartupProject API
- Revert the very first migration: update-database 0 -StartupProject API -Project Infrastructure
- Remove migration: remove-migration -StartupProject API -Project Infrastructure
---

## Database & Migrations
- Default database: `Skinet`
- Azure SQL Server example:
  - Server name: skinet-server-name
  - Server admin login: appuser
  - Password: Pa$$w0rd

---

## Redis Setup

### Local Redis (via Docker Compose): docker compose up -d
### Upstash Example Redis: "<host>:<port>,password=<password>,ssl=True,abortConnect=False"
- "Redis": "just-asp-27676.upstash.io:6379,password=AWwcAAIjcDFlMDAzYTRhYjUwN2I0NDJiYjM1NTI1YWJmNWJkZWQ3MnAxMA,ssl=True,abortConnect=False"
### Azure Redis Example Redis: "<azure-redis-host>:6380,password=<password>,ssl=True,abortConnect=False"
- "Redis": "SkinetTest.redis.cache.windows.net:6380,password=nZCeSltOMQGqR87fO8pANdfSSbWIVqD3NAzCaF38id4=,ssl=True,abortConnect=False"
### Connect via CLI: redis-cli --tls -u redis://default:<password>@<host>:6379
- redis-cli --tls -u redis://default:AWwcAAIjcDFlMDAzYTRhYjUwN2I0NDJiYjM1NTI1YWJmNWJkZWQ3MnAxMA@just-asp-27676.upstash.io:6379
---

## Stripe Integration

### 1. Login to Stripe CLI 
- stripe login
- Your pairing code is: likes-rosy-comfy-skill
- This pairing code verifies your authentication with Stripe.
- Press Enter to open the browser or visit https://dashboard.stripe.com/stripecli/confirm_auth?t=WR1LrC9pJPd0TqoIQo5GIP4Ik7lZNdPR
### 2. Listen for Webhook Events
- Generic event: stripe listen --forward-to https://localhost:7130/api/payments/webhook
- Specific event: stripe listen --forward-to https://localhost:7130/api/payments/webhook -e payment_intent.succeeded
- You have not configured API keys yet. Running `stripe login`...
- Your pairing code is: lucid-great-hooray-talent
- This pairing code verifies your authentication with Stripe.
- Press Enter to open the browser or visit https://dashboard.stripe.com/stripecli/confirm_auth?t=j1oC1pn5DdyHqlqyZuuHTSziiSRAoBux
- Done! The Stripe CLI is configured for New business sandbox with account id acct_1RT0O9PRLySQ1YmY
### 3. Webhook Secret
- Use your Stripe dashboard or CLI output to get the webhook signing secret.
- Run this command to test strpie locally: stripe listen --forward-to https://localhost:7130/api/payments/webhook -e payment_intent.succeeded
- Ready! You are using Stripe API Version [2025-04-30.basil]. Your webhook signing secret is whsec_97a59df44d7bdee883a100e614854673fba09a5dcc296adae7b1c7ca061c0a44

---

## Sample Users

| Role      | Email           | Password  |
|-----------|-----------------|-----------|
| Customer  | tom@test.com    | Pa$$w0rd  |
| Admin     | admin@test.com  | Pa$$w0rd  |

---

## Azure Deployment
- Update connection strings and secrets in your Azure App Service configuration.
- Use the provided Redis and SQL connection string formats above.

---

## Notes
- Replace all placeholder values (e.g., `<password>`, `<host>`, `<your-server-name>`) with your actual credentials.
- Do not commit sensitive credentials to version control.

---

## License
MIT