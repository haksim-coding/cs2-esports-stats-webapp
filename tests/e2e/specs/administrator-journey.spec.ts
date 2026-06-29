import { expect, type Page, test } from '@playwright/test';

const adminUsername = process.env.PW_ADMIN_USERNAME ?? 'admin_maksim';
const adminPassword = process.env.PW_ADMIN_PASSWORD ?? 'password123';

type CreatedRecords = {
  teamADeletePath?: string;
  teamBDeletePath?: string;
  playerDeletePath?: string;
  eventDeletePath?: string;
  matchDetailsPath?: string;
};

async function submitCreateForm(page: Page) {
  await Promise.all([
    page.waitForURL((url) => !url.pathname.toLowerCase().endsWith('/create'), { timeout: 10_000 }),
    page.getByRole('button', { name: 'Create', exact: true }).click(),
  ]);
}

async function deleteRecord(page: Page, deletePath: string | undefined, buttonName: string) {
  if (!deletePath) return;

  try {
    const response = await page.goto(deletePath, { waitUntil: 'domcontentloaded' });
    if (!response || response.status() >= 400 || !page.url().toLowerCase().includes('/delete/')) return;

    const button = page.getByRole('button', { name: buttonName, exact: true });
    if (await button.isVisible()) {
      await button.click();
      await page.waitForLoadState('domcontentloaded');
    }
  } catch {
    // Cleanup is best-effort so it cannot hide the journey's original failure.
  }
}

let created: CreatedRecords = {};

test.afterEach(async ({ page }) => {
  await deleteRecord(page, created.eventDeletePath, 'Delete');
  await deleteRecord(page, created.playerDeletePath, 'Delete');
  await deleteRecord(page, created.teamADeletePath, 'Delete Team');
  await deleteRecord(page, created.teamBDeletePath, 'Delete Team');
});

test('administrator manages a complete event lifecycle', async ({ page }) => {
  const suffix = `${Date.now().toString(36)}-${process.pid}`;
  const teamA = `E2E Alpha ${suffix}`;
  const teamB = `E2E Bravo ${suffix}`;
  const player = `e2e-${suffix}`;
  const event = `E2E Cup ${suffix}`;
  created = {};

  await test.step('1. Open the application', async () => {
    await page.goto('/');
    await expect(page).toHaveTitle(/CS2Scope/i);
  });

  await test.step('2. Log in as administrator', async () => {
    await page.getByRole('link', { name: 'Login', exact: true }).click();
    await page.getByLabel('Username').fill(adminUsername);
    await page.getByLabel('Password').fill(adminPassword);
    await page.getByRole('button', { name: 'Login', exact: true }).click();

    const navigation = page.getByRole('navigation');
    await expect(navigation.getByText(adminUsername, { exact: true })).toBeVisible();
    await expect(navigation.getByText('Admin', { exact: true })).toBeVisible();
  });

  await test.step('3. Create Team A', async () => {
    await page.goto('/teams/create');
    await page.getByLabel('Name', { exact: true }).fill(teamA);
    await page.getByLabel('Tag', { exact: true }).fill(`A${suffix.slice(-7)}`);
    await page.getByLabel('Country Code').fill('HR');
    await page.getByLabel('Year Founded').fill('2026');
    await submitCreateForm(page);

    created.teamADeletePath = new URL(page.url()).pathname.replace(/\/details\//i, '/delete/');
    await expect(page.getByRole('heading', { name: teamA, exact: true })).toBeVisible();
  });

  await test.step('4. Create Team B', async () => {
    await page.goto('/teams/create');
    await page.getByLabel('Name', { exact: true }).fill(teamB);
    await page.getByLabel('Tag', { exact: true }).fill(`B${suffix.slice(-7)}`);
    await page.getByLabel('Country Code').fill('SE');
    await page.getByLabel('Year Founded').fill('2026');
    await submitCreateForm(page);

    created.teamBDeletePath = new URL(page.url()).pathname.replace(/\/details\//i, '/delete/');
    await expect(page.getByRole('heading', { name: teamB, exact: true })).toBeVisible();
  });

  await test.step('5. Create a player and assign them to Team A', async () => {
    await page.goto('/players/create');
    await page.getByLabel('Nickname').fill(player);
    await page.getByLabel('Full Name').fill('Playwright Demo Player');
    await page.getByLabel('Country Code').fill('HR');
    await page.getByLabel('Role').selectOption({ label: 'Rifler' });
    await page.getByLabel('Current Team').selectOption({ label: teamA });
    await page.getByLabel('Rating 2.0').fill('1.25');
    await page.getByLabel('Total Maps Played').fill('42');
    await submitCreateForm(page);

    created.playerDeletePath = await page.getByRole('link', { name: 'Delete', exact: true }).getAttribute('href') ?? undefined;
    await expect(page.getByRole('heading', { name: player, exact: true })).toBeVisible();
    await expect(page.locator('.player-chip--team', { hasText: teamA })).toBeVisible();
  });

  await test.step('6. Create an event containing both teams', async () => {
    await page.goto('/events/create');
    await page.getByLabel('Event Name').fill(event);
    await page.getByLabel('Organizer').fill('Playwright E2E');
    await page.getByLabel('Prize Pool').fill('250000');
    await page.getByLabel('LAN Event').check();

    const teamSearch = page.locator('[data-team-search-input]');
    for (const teamName of [teamA, teamB]) {
      await teamSearch.fill(teamName);
      await page.locator('[data-team-result]', { hasText: teamName }).click();
    }

    await expect(page.locator('input[name="SelectedTeamIds"]')).toHaveCount(2);
    await submitCreateForm(page);
    created.eventDeletePath = await page.getByRole('link', { name: 'Delete', exact: true }).getAttribute('href') ?? undefined;
    await expect(page.getByRole('heading', { name: event, exact: true })).toBeVisible();
    await expect(page.getByText(teamA, { exact: true })).toBeVisible();
    await expect(page.getByText(teamB, { exact: true })).toBeVisible();
  });

  await test.step('7. Create a match for that event', async () => {
    await page.goto('/matches/create');
    await page.getByLabel('Event').selectOption({ label: event });
    await page.getByLabel('Team A').first().selectOption({ label: teamA });
    await page.getByLabel('Team B').first().selectOption({ label: teamB });
    await page.getByLabel('Format').selectOption({ label: 'BestOf3' });

    const mapRows = page.locator('[data-match-map-row]');
    await mapRows.nth(0).locator('[data-match-map-select]').selectOption({ label: 'Ancient' });
    await mapRows.nth(0).locator('[data-match-map-score-a]').fill('13');
    await mapRows.nth(0).locator('[data-match-map-score-b]').fill('8');
    await mapRows.nth(1).locator('[data-match-map-select]').selectOption({ label: 'Mirage' });
    await mapRows.nth(1).locator('[data-match-map-score-a]').fill('13');
    await mapRows.nth(1).locator('[data-match-map-score-b]').fill('10');
    await submitCreateForm(page);

    await expect(page.getByRole('heading', { name: /Match #/ })).toBeVisible();
    created.matchDetailsPath = new URL(page.url()).pathname;
  });

  await test.step('8. Edit the match score', async () => {
    await page.getByRole('link', { name: 'Edit', exact: true }).click();
    const secondMap = page.locator('[data-match-map-row]').nth(1);
    await secondMap.locator('[data-match-map-score-b]').fill('7');
    await page.getByRole('button', { name: 'Save changes', exact: true }).click();
    await page.waitForURL(/\/matches\/details\/\d+/i);
  });

  await test.step('9. Verify the updated information on the details page', async () => {
    await expect(page.getByRole('heading', { name: new RegExp(`${teamA}\\s+VS\\s+${teamB}`, 'i') })).toBeVisible();
    await expect(page.getByRole('link', { name: event, exact: true }).first()).toBeVisible();

    const mirage = page.locator('[data-map="Mirage"]');
    await expect(mirage).toContainText('13');
    await expect(mirage).toContainText('7');
    await expect(page.locator('[data-map]')).toHaveCount(2);
  });

  await test.step('10. Delete the created records and verify they disappear', async () => {
    await deleteRecord(page, created.eventDeletePath, 'Delete');
    created.eventDeletePath = undefined;
    await deleteRecord(page, created.playerDeletePath, 'Delete');
    created.playerDeletePath = undefined;
    await deleteRecord(page, created.teamADeletePath, 'Delete Team');
    created.teamADeletePath = undefined;
    await deleteRecord(page, created.teamBDeletePath, 'Delete Team');
    created.teamBDeletePath = undefined;

    const deletedMatch = await page.request.get(created.matchDetailsPath!);
    expect(deletedMatch.status()).toBe(404);

    await page.goto('/events');
    await expect(page.getByText(event, { exact: true })).toHaveCount(0);
    await page.goto('/players');
    await expect(page.getByText(player, { exact: true })).toHaveCount(0);
    await page.goto('/teams');
    await expect(page.getByText(teamA, { exact: true })).toHaveCount(0);
    await expect(page.getByText(teamB, { exact: true })).toHaveCount(0);
  });
});
