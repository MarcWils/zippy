/// <binding BeforeBuild='Profile - Development' />
'use strict';
const path = require('path');

module.exports = {
    devtool: 'source-map',
    entry: {
        app: ['./scripts/app.ts']
    },
    output: {
        filename: 'site.js',
        path: path.resolve(__dirname, 'wwwroot/dist/')
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                exclude: '/node_modules/',
                use: [
                    {
                        loader: 'ts-loader'
                    }
                ]
            }
        ]
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js']
    },
    externals: {
    }
};
