function StatisticRevenue(idBranch) {
    $('#modal_statistic').modal('show');
    $.ajax({
        url: '/Statistic/GetRevenueBranch7DaysById',
        type: 'GET',
        data: { idBranch: idBranch },
        success: function (response) {

            // Lấy key và value từ dictionary
            var labels = Object.keys(response); // Lấy danh sách ngày
            var dataPoints = Object.values(response); // Lấy danh sách doanh thu

            // Chuẩn bị dữ liệu cho biểu đồ
            var data = {
                labels: labels, // Ngày
                datasets: [{
                    label: 'Doanh thu (VND)',
                    data: dataPoints, // Doanh thu
                    backgroundColor: 'rgba(54, 162, 235, 0.2)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                }]
            };

            // Cấu hình biểu đồ
            var options = {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            };

            // Render biểu đồ
            var ctx = document.getElementById('statisticChart').getContext('2d');

            // Kiểm tra và hủy biểu đồ cũ nếu tồn tại
            if (window.statisticChart && typeof window.statisticChart.destroy === 'function') {
                window.statisticChart.destroy();
            }

            // Tạo biểu đồ mới
            window.statisticChart = new Chart(ctx, {
                type: 'bar',
                data: data,
                options: options
            });
        },
        error: function (err) {
            console.error("Error fetching data:", err);
        }
    });
}
