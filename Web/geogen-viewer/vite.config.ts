import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// Static-deploy friendly: relative base path so `dist/` works from any directory or CDN.
export default defineConfig({
  plugins: [react()],
  base: "./",
  build: {
    outDir: "dist",
    sourcemap: true,
  },
});
