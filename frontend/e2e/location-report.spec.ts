import { expect, test } from "@playwright/test";

const EMAIL    = process.env.E2E_EMAIL;
const PASSWORD = process.env.E2E_PASSWORD;

// Kızılay, Ankara: a central point where OSRM always finds a road network.
const ADDRESS = { place_id: 1, display_name: "Kızılay, Çankaya, Ankara", lat: "39.92077", lon: "32.85411" };

test("enter address → report appears", async ({ page }) => {
  if (!EMAIL || !PASSWORD) throw new Error("Set E2E_EMAIL and E2E_PASSWORD (user with location_analysis permission).");

  // The public geocoder is mocked so rate limits or result changes cannot break the test.
  await page.route("https://nominatim.openstreetmap.org/search**", (route) => route.fulfill({ json: [ADDRESS] }));

  await test.step("log in", async () => {
    await page.goto("/login");
    await page.getByLabel("E-posta").fill(EMAIL);
    await page.getByLabel("Şifre").fill(PASSWORD);
    await page.getByRole("button", { name: "Giriş Yap" }).click();
    await expect(page).toHaveURL(/\/dashboard/);
  });

  await test.step("open location report", async () => {
    // The navbar always starts collapsed behind "Menü".
    await page.getByRole("button", { name: "Menü" }).click();
    await page.getByRole("button", { name: "Analiz" }).click();
    await page.getByRole("button", { name: "Konum Raporu" }).click();
  });

  await test.step("pick address", async () => {
    await page.getByRole("button", { name: "Adres Ara" }).click();
    await page.getByPlaceholder("Adres veya konum ara...").fill("Kızılay");
    await page.getByRole("button", { name: "Ara", exact: true }).click();
    await page.getByRole("button", { name: ADDRESS.display_name }).click();
  });

  await test.step("choose categories and create report", async () => {
    // Two different root categories, so the competitor never overlaps a complement's subtree.
    const competitor = page.getByLabel("Rakip kategori");
    await expect(competitor.locator("option")).not.toHaveCount(1);
    const rootIds: string[] = [];
    for (const option of await competitor.locator("option:not([disabled])").all()) {
      const value = await option.getAttribute("value");
      if (value && !(await option.textContent())?.startsWith("—")) rootIds.push(value);
    }
    expect(rootIds.length, "need at least two root POI categories").toBeGreaterThanOrEqual(2);
    await competitor.selectOption(rootIds[0]);
    await page.getByLabel("Tamamlayıcı kategori ekle").selectOption(rootIds[1]);

    const responsePromise = page.waitForResponse((r) => r.url().endsWith("/location-report"));
    await page.getByRole("button", { name: "Rapor Oluştur" }).click();
    const response = await responsePromise;

    if (!response.ok()) {
      const body = await response.text();
      if (response.status() === 409 && body.includes("hizmet alanı hesaplanamadı"))
        throw new Error(`Report failed: no isochrone at this point (no road nearby, or OSRM unavailable): ${body}`);
      throw new Error(`Location report request failed: ${response.status()} ${body}`);
    }

    await expect(page.getByText("/ 100")).toBeVisible();
    for (const minutes of [5, 10, 15])
      await expect(page.getByRole("cell", { name: `${minutes} dk`, exact: true })).toBeVisible();
  });
});
