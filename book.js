var pkg = require('./package.json');

module.exports = {
    root: './docs',
    title: 'NuClear River Documentation',

    // Enforce use of GitBook v3
    gitbook: '>=3.0.0-pre.0',

    // Use the "official" theme
    plugins: ['theme-official'],

    variables: {
        version: pkg.version
    },
};