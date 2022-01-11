module.exports = {
  content: ["./Views/**/*.{cshtml,js,html}", "./React/*.jsx"],
  theme: {
    extend: {},
  },
  plugins: [
    require('@tailwindcss/forms'),
    require('daisyui'),
  ],
}
