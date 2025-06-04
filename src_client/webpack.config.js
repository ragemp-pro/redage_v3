const path = require("path");

const LIBRARY_NAME = 'redage-clientside';
const OUTPUT_FILE = 'main.js';

module.exports = (env, argv) => {
    const mode = argv.mode || 'development';
    const isProduction = mode === 'production';

    return {
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
            minimize: isProduction
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
        },
        devtool: isProduction ? false : 'source-map'
    }
};