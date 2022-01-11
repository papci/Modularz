const path = require('path')
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = {
    // Where files should be sent once they are bundled
    entry: './React/app.jsx',
    output: {
        filename: 'main.js',
        path: path.resolve(__dirname, 'wwwroot/app'),
    },
    // Rules of how webpack will take our files, complie & bundle them for the browser 
    module: {
        rules: [
            {
                test: /\.(jsx)$/,
                include: path.resolve(__dirname, 'React/'),
                use: {
                    loader: 'babel-loader'
                }
            },
            {
                test: /\.css$/i,
                use: [MiniCssExtractPlugin.loader, "css-loader", "postcss-loader"],
            }
            
        ]
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: "app.css",
        }),
    ]

}
