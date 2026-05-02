import React from "react";
import ReactDOM from "react-dom/client";
import { MantineProvider, createTheme } from "@mantine/core";
import { App } from "./App";
// Mantine's stylesheet must come BEFORE our local styles so our overrides win.
import "@mantine/core/styles.css";
import "./styles/colors.css";
import "./styles/report.css";

// Theme tuned for a more contemporary look: indigo primary (bolder than the default blue,
// pairs well with the multi-color theorem palette without competing with it), softer 8px
// default radius across all components, and a modern system-font stack. We crank
// `defaultGradient` so the few places that use Mantine gradients (e.g., the upload-zone
// background hover) feel intentional rather than flat.
const theme = createTheme({
  primaryColor: "indigo",
  defaultRadius: "md",
  autoContrast: true,
  fontFamily:
    'Inter, -apple-system, BlinkMacSystemFont, "Segoe UI", system-ui, sans-serif',
  fontFamilyMonospace:
    "ui-monospace, SFMono-Regular, Menlo, monospace",
  headings: {
    fontFamily:
      'Inter, -apple-system, BlinkMacSystemFont, "Segoe UI", system-ui, sans-serif',
    fontWeight: "600",
  },
  defaultGradient: { from: "indigo", to: "violet", deg: 135 },
  components: {
    Paper: {
      defaultProps: { shadow: "xs", radius: "md", withBorder: true },
    },
    Card: {
      defaultProps: { shadow: "sm", radius: "md", withBorder: true },
    },
    Button: {
      defaultProps: { radius: "md" },
    },
    Badge: {
      defaultProps: { radius: "sm" },
    },
    TextInput: {
      defaultProps: { radius: "md" },
    },
  },
});

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <MantineProvider theme={theme}>
      <App />
    </MantineProvider>
  </React.StrictMode>,
);
