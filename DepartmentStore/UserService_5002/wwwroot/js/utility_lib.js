function ShowToastNoti(type, title, message, timeout = 2000, position = 'topRight', icon = '') {
    let color = '';
    let audio = document.getElementById('audio_info');
    switch (type) {
        case 'success': icon = '<i class="iziToast-icon ico-success revealIn"></i>'; color = 'green';
        //var audio = new Audio('/sounds/sound1.ogg'); audio.volume = 0.2; audio.play(); break;
        case 'warning': icon = '<i class="iziToast-icon ico-warning revealIn"></i>'; color = 'orange';
        //var audio = new Audio('/sounds/sound6.ogg'); audio.volume = 0.8; audio.play(); break;
        case 'error': icon = '<i class="iziToast-icon ico-error revealIn"></i>'; color = 'red';
        //var audio = new Audio('/sounds/sound5.ogg'); audio.volume = 0.4; audio.play(); break;
        case 'info': icon = '<i class="iziToast-icon ico-info revealIn"></i>'; color = 'blue';
        //var audio = new Audio('/sounds/sound7.ogg'); audio.volume = 0.4; audio.play(); break;
        case 'question': icon = '<i class="iziToast-icon ico-question revealIn"></i>'; color = 'yellow';
        //var audio = new Audio('/sounds/sound4.ogg'); audio.volume = 0.4; audio.play(); break;
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

