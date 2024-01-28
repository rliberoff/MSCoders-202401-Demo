const path = require("path")
const webpack = require('webpack');
const webpackUtf8Bom = require('webpack-utf8-bom');
const webpackCopyPlugin = require("copy-webpack-plugin");

const { merge } = require('webpack-merge');

console.log(`\nLoading ".${process.env.NODE_ENV}.env"\n`)

require('dotenv').config({ path: path.resolve(__dirname, `./env/.${process.env.NODE_ENV}.env`) });

const local = {
    devtool: 'inline-source-map',
    devServer: {
        devMiddleware: {
            writeToDisk: true,
        },
    },
    mode: 'development'
};

const development = {
    devtool: 'inline-source-map',
    devServer: {
        devMiddleware: {
            writeToDisk: true,
        },
    },
    mode: 'development'
};

const test = {
    devtool: 'inline-source-map',
    devServer: {
        devMiddleware: {
            writeToDisk: true,
        },
    },
    mode: 'development'
};

const production = {
    mode: 'production'
};

const envConfig = () => {
    switch (process.env.NODE_ENV) {
        case "local":
            return local;

        case "development":
            return development;

        case "test":
            return test;

        case "production":
            return production;

        default:
            throw new Error(`Invalid NODE_ENV: ${process.env.NODE_ENV}`);
    }
};

function replaceEnvironmentVariables(content) {
    return content.toString().replace(/\{\{(\$\w+)\}\}/g, (match, variable) => {
        const envVariable = variable.substring(1);
        return process.env[envVariable] || match;
    });
}

module.exports = merge(envConfig(), {
    entry: path.resolve(__dirname, './src/bot-webchat.js'),
    output: {
        clean: true,
        filename: 'bot-webchat.js',
        path: path.resolve(__dirname, './dist'),
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                use: {
                    loader: 'webpack-replace-loader',
                    options: {
                      search: '${{WEB_CHAT_PREFIX}}',
                      replace: process.env.WEB_CHAT_PREFIX,
                    },
                  },
            },
        ]
    },
    plugins: [   
        new webpackCopyPlugin({
            patterns: [
                { from: path.resolve(__dirname, 'assets'), to: './assets' },
                { from: path.resolve(__dirname, 'settings'), to: '.', transform(content) { return replaceEnvironmentVariables(content); } }
            ]
        }),
        new webpackUtf8Bom(true),
        new webpack.EnvironmentPlugin([
            'DIRECT_LINE_DOMAIN',
            'DIRECT_LINE_TOKEN',
            'LOCAL_STORAGE_PREFIX',
            'WEB_CHAT_PREFIX',
            'ASSETS_BASE_URL'
        ])
    ],
});
