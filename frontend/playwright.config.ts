import { defineConfig, devices } from "@playwright/test";

// Runs against the real local stack: the backend (localhost:5130) must already be running.
export default defineConfig({
  testDir: "./e2e",
  timeout: 90_000,
  use: {
    baseURL: "http://localhost:5173",
    trace:   "retain-on-failure",
  },
  // Desktop is the main use case.
  projects: [{ name: "chromium", use: { ...devices["Desktop Chrome"], viewport: { width: 1920, height: 1080 } } }],
  webServer: {
    command:             "npm run dev",
    url:                 "http://localhost:5173",
    reuseExistingServer: true,
  },
});
