const path = require("path");

const LIBRARY_NAME = 'redage-clientside';
const OUTPUT_FILE = 'main.js';

module.exports = mode => ({
    entry: './index.js',
    mode,
    /*module: {
        rules: [
            {
                test: /\.js$/,
                exclude: /node_modules/,
                loader: 'babel-loader',

            }
        ]
    },*/
    optimization: {
        minimize: true
    },
    performance: {
        hints: false
    },
    output: {
		//path: path.join(__dirname, "public"),
        path: path.join(__dirname, "../client_packages"),
        filename: OUTPUT_FILE,
        library: LIBRARY_NAME,
        libraryTarget: 'umd',
        globalObject: 'global',
    }
});