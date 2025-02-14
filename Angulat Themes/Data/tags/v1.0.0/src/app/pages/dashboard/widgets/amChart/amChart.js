'use strict';

blurAdminApp.directive('amChart', function () {
  return {
    restrict: 'E',
    controller: ['$scope', 'tplSkinManager', 'tplSkinChartWatcherHelper', function ($scope, tplSkinManager, tplSkinChartWatcherHelper) {
      var chartData = [
        { date: new Date(2012, 11), value: 0, value0: 0 },
        { date: new Date(2013, 0), value: 15000, value0: 19000},
        { date: new Date(2013, 1), value: 30000, value0: 20000},


        { date: new Date(2013, 2), value: 25000, value0: 22000},
        { date: new Date(2013, 3), value: 21000, value0: 25000},
        { date: new Date(2013, 4), value: 24000, value0: 29000},
        { date: new Date(2013, 5), value: 31000, value0: 26000},
        { date: new Date(2013, 6), value: 40000, value0: 25000},
        { date: new Date(2013, 7), value: 37000, value0: 20000},
        { date: new Date(2013, 8), value: 18000, value0: 22000},
        { date: new Date(2013, 9), value: 5000, value0: 26000},
        { date: new Date(2013, 10), value: 40000, value0: 30000},
        { date: new Date(2013, 11), value: 20000, value0: 25000},
        { date: new Date(2014, 0), value: 5000, value0: 13000},

        { date: new Date(2014, 1), value: 3000, value0: 13000},
        { date: new Date(2014, 2), value: 1800, value0: 13000},
        { date: new Date(2014, 3), value: 10400, value0: 13000},
        { date: new Date(2014, 4), value: 25500, value0: 13000},
        { date: new Date(2014, 5), value: 2100, value0: 13000},
        { date: new Date(2014, 6), value: 6500, value0: 13000},
        { date: new Date(2014, 7), value: 1100, value0: 13000},
        { date: new Date(2014, 8), value: 17200, value0: 13000},
        { date: new Date(2014, 9), value: 26900, value0: 13000},
        { date: new Date(2014, 10), value: 14100, value0: 13000},
        { date: new Date(2014, 11), value: 35300, value0: 13000},
        { date: new Date(2015, 0), value: 54800, value0: 13000},
        { date: new Date(2015, 1), value: 49800, value0: 13000}
      ];

      var chartColorProfile = tplSkinManager.getChartColorProfile();

      var chart = AmCharts.makeChart('amchart', {
        type: 'serial',
        theme: 'blur',
        marginTop: 15,
        marginRight: 15,
        dataProvider: chartData,
        categoryField: 'date',
        categoryAxis: {
          parseDates: true,
          gridAlpha: 0,
          color: chartColorProfile.fontColors,
          axisColor: chartColorProfile.axisColors
        },
        valueAxes: [
          {
            minVerticalGap: 50,
            gridAlpha: 0,
            color: chartColorProfile.fontColors,
            axisColor: chartColorProfile.axisColors
          }
        ],
        graphs: [
          {
            id: 'g0',
            bullet: 'none',
            useLineColorForBulletBorder: true,
            lineColor: colorDefault,
            lineThickness: 1,
            negativeLineColor: colorDanger,
            type: 'smoothedLine',
            valueField: 'value0',
            fillAlphas: 1,
            fillColorsField: 'lineColor'
          },
          {
            id: 'g1',
            bullet: 'none',
            useLineColorForBulletBorder: true,
            lineColor: colorPrimary,
            lineThickness: 1,
            negativeLineColor: colorDanger,
            type: 'smoothedLine',
            valueField: 'value',
            fillAlphas: 1,
            fillColorsField: 'lineColor'
          }
        ],
        chartCursor: {
          categoryBalloonDateFormat: 'MM YYYY',
          categoryBalloonColor: '#4285F4',
          categoryBalloonAlpha: 0.7,
          cursorAlpha: 0,
          valueLineEnabled: true,
          valueLineBalloonEnabled: true,
          valueLineAlpha: 0.5
        },
        dataDateFormat: 'MM YYYY',
        export: {
          enabled: true
        },
        creditsPosition: 'bottom-right',
        zoomOutButton: {
          backgroundColor: '#fff',
          backgroundAlpha: 0
        },
        zoomOutText: '',
        pathToImages: 'img/'
      });

      tplSkinChartWatcherHelper.watchAxisChartStyleChanges($scope, chart);

      function zoomChart() {
        chart.zoomToDates(new Date(2013, 3), new Date(2014, 0));
      }

      chart.addListener('rendered', zoomChart);
      zoomChart();
      if (chart.zoomChart) {
        chart.zoomChart();
      }
    }],
    templateUrl: 'app/pages/dashboard/widgets/amChart/amChart.html'
  };
});