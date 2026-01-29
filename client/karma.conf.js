module.exports = function (config) {
  config.set({
    basePath: '',
    frameworks: ['jasmine', '@angular-devkit/build-angular'],
    plugins: [
      require('karma-jasmine'),
      require('karma-chrome-launcher'),
      require('karma-junit-reporter'),
      require('karma-jasmine-html-reporter'),
      require('karma-coverage'),
      require('@angular-devkit/build-angular/plugins/karma')
    ],
    client: {
      jasmine: {
        random: false
      },
    },
    jasmineHtmlReporter: {
      suppressAll: true
    },

    junitReporter: {
      outputDir: 'reports',
      outputFile: 'junit.xml',
      useBrowserName: false
    },

    coverageReporter: {
      dir: 'reports',
      subdir: '.',
      reporters: [
        { type: 'html' },
        { type: 'cobertura' },
        { type: 'text-summary' }
      ]
    },

    reporters: ['progress', 'kjhtml', 'junit', 'coverage'],

    browsers: ['Chrome'],
    restartOnFileChange: true,
    singleRun: false
  });
};
