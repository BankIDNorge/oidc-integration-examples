/** @type {import("tailwindcss").Config} */
module.exports = {
  content: [
    "./src/pages/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/components/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/app/**/*.{js,ts,jsx,tsx,mdx}"
  ],
  theme: {
    extend: {
      colors: {
        "background-secondary": "#161225",
        "background-light": "#39134C",
        "background-light-hover": "#470D70",
        "text-light": "#FFFFFF",
        "background-dark": "#F6F6F9",
        "background-dark-hover": "#EFEEF3",
        "text-dark": "#39134C",
        "background-green": "#4DFE95",
        "text-highlight": "#000000"
      }
    }
  },
  plugins: []
};
