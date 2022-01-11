const purgecss = require('@fullhuman/postcss-purgecss')
module.exports = {
    plugins: [
        require('postcss-import'),
        require('tailwindcss/nesting'),
        require('tailwindcss'),
        require('autoprefixer'),
        require('cssnano'),
        purgecss({
            content: ["./Views/**/*.{cshtml,js,html}", "./React/*.jsx"],
            defaultExtractor: content => content.match(/[\w-/:]+(?<!:)/g) || []
        })
    ]
}