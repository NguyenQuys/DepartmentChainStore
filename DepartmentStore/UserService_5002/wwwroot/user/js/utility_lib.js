function ShowToastNoti(type, title, message, timeout = 2000, position = 'topRight', icon = '') {
    let color = '';
    let audio = document.getElementById('audio_info');
    switch (type) {
        case 'success': icon = '<i class="iziToast-icon ico-success revealIn"></i>'; color = 'green';
            break;
        case 'warning': icon = '<i class="iziToast-icon ico-warning revealIn"></i>'; color = 'orange';
            break;
        case 'error': icon = '<i class="iziToast-icon ico-error revealIn"></i>'; color = 'red';
            break;
        case 'info': icon = '<i class="iziToast-icon ico-info revealIn"></i>'; color = 'blue';
            break;
        case 'question': icon = '<i class="iziToast-icon ico-question revealIn"></i>'; color = 'yellow';
            break;
        default: break;
    }
    iziToast.show({
        icon: icon,
        color: color,
        displayMode: 0,
        title: title,
        message: message,
        position: position,
        progressBar: true,
        timeout: timeout,
        transitionIn: 'flipInX',
        transitionOut: 'flipOutX',
        progressBarColor: 'rgb(0, 255, 184)',
        imageWidth: 70,
        layout: 2,
        iconColor: 'rgb(0, 255, 184)'
    });
}

