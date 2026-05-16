const data = window.analyticsData;

new Chart(document.getElementById('barChart'), {
    type: 'bar',
    data: {
        labels: data.categoryLabels,
        datasets: [{
            label: 'Complaints',
            data: data.categoryCounts,
            backgroundColor: 'rgba(179, 29, 29, 1)',
            borderColor: 'rgba(179, 29, 29, 1)',
            borderWidth: 1
        }]
    }
});

new Chart(document.getElementById('lineChart'), {
    type: 'line',
    data: {
        labels: data.monthLabels,
        datasets: [{
            label: 'Complaints',
            data: data.monthCounts,
            borderColor: 'rgba(179, 29, 29, 1)',
            backgroundColor: 'rgba(179, 29, 29, 0.2)',
            tension: 0.3,
            fill: true,
            pointBackgroundColor: 'rgba(179, 29, 29, 1)'
        }]
    }
});