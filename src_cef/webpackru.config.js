const path = require('path');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const HtmlWebpackPlugin = require('html-webpack-plugin');

const mode = process.env.NODE_ENV || 'development';
const prod = mode === 'production';

const urlPath = `https://cdn.ragemp.pro/`

module.exports = {
    entry: {
        main: './src/main.js'
    },
    resolve: {
        alias: {
            "svelte": path.resolve('node_modules', 'svelte'),
            "@": path.resolve(__dirname, './src'),
            "api": path.resolve(__dirname, './src/api'),
            "store": path.resolve(__dirname, './src/store'),
            "components": path.resolve(__dirname, './src/components'),
            "router": path.resolve(__dirname, './src/router/index.js'),
            "json": path.resolve(__dirname, './src/json'),
            "lang": path.resolve(__dirname, './lang')
        },
    },
    output: {
        //path: path.resolve(__dirname, 'public'),
        path: path.resolve(__dirname, "./../client_packages/interface"),
		filename: `buildru/bundle.js`,
        libraryTarget: "umd",
    },
    plugins: [
        new HtmlWebpackPlugin({
            template: "./src/index.html",
            title: `RedAge - ${new Date()}`,
            filename: './index.html',
            inject: false,
        }),
        new MiniCssExtractPlugin({ 
            filename: `buildru/bundle.css`
        })
    ],
    module: {
        rules: [
            {
				test: /\.svelte$/,
				use: {
					loader: 'svelte-loader',
					options: {
						hotReload: true
					}
				}
            },
            {
                test: /\.(c|sac|sa|sc)ss$/i,
                enforce: "pre",
                use: [
                    {
                        loader: MiniCssExtractPlugin.loader, //4. Extract css into files\
                        options: {
                            publicPath: '../', // опускаемся из build директории
                        }
                    },
                    "css-loader", { // 3 Turns css into javascript
                        loader: "postcss-loader", //2. Runs Autoprefixer
                        options: {
                            ident: "postcss",
                            plugins: [require("autoprefixer")]
                        }
                    },
                    "sass-loader" // 1. Turns sass into css
                ]
            },
            // {
                // test: /\.(jpe?g|png|svg?|gif)$/i,
                // use: [{
                    // loader: 'file-loader',
                    // options: {
                        // esModule: false,
                        // name: '[path]/[name].[ext]',
                        // publicPath: (url, resourcePath, context) => {
                            // url = url.split('/').filter(x => x).join('/');
                            // return urlPath + url;
                        // }
                    // }
                // }]
            // },
            {
                test: /\.(webm|ttf|eot|woff(2)?|ogg|mp3|wav|mpe?g)(\?[a-z0-9=&.]+)?$/,
                use: [{
                    loader: 'file-loader',
                    options: {
                        name: '[path]/[name].[ext]'
                    }
                }]
			},
        ]
    },
    mode,
    devtool: prod ? false: 'source-map',
    devServer: {
        contentBase: path.join(__dirname, 'dist'),
		inline: true,
        compress: true,
        port: 8888
    }
}