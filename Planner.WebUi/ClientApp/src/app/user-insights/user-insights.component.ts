import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-user-insights',
  templateUrl: './user-insights.component.html',
  styleUrls: ['./user-insights.component.scss']
})
export class UserInsightsComponent implements OnInit {
  occupancyOptions = {
    color: ['#ee6666', '#dddddd', '#fac858', '#73c0de', '#3ba272', '#fc8452', '#9a60b4', '#ea7ccc'],
    tooltip: {
      trigger: 'item'
    },
    legend: null,
    series: [
      {
        name: 'Occupancy',
        type: 'pie',
        radius: ['47%', '70%'],
        avoidLabelOverlap: false,
        label: {
          show: false,
          position: 'center'
        },
        tooltip: {
          show: false
        },
        emphasis: {
          label: {
            show: false,
            fontSize: '16',
            fontWeight: 'bold'
          }
        },
        labelLine: {
          show: false
        },
        data: [
          { value: 60, name: 'Occupied' },
          { value: 40, name: 'Unoccupied' },
        ]
      }
    ]
  };

  constructor(
  ) { }

  ngOnInit(): void {
  }
}
