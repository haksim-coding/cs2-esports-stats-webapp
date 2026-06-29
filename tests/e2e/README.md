# Playwright end-to-end journey

This suite contains one ten-step administrator journey that creates two teams, a player, an event, and a match; updates and verifies the score; then removes all created data.

## Run it

From the repository root:

```powershell
docker compose up -d
dotnet ef database update
cd tests/e2e
npm install
npx playwright install chromium
npm test
```

The Playwright configuration starts the ASP.NET Core application on `http://127.0.0.1:5180`. To test an application that is already running, set `PLAYWRIGHT_BASE_URL`.

The scenario defaults to the seeded `admin_maksim` / `password123` administrator. Override those values with `PW_ADMIN_USERNAME` and `PW_ADMIN_PASSWORD`.
