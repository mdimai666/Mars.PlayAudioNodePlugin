# PlayAudioNodePlugin

<p align="center">
    <img src="assets/icon.png" alt="logo" />
</p>

Mars.PlayAudioNodePlugin - provide play audio functions for Mars
- Play audio
- List output devices

> Audio provider is [NAudio](https://github.com/naudio/NAudio/)

# Screenshot

<p align="center">
    <img src="assets/screenshot1.png" alt/>
</p>

## Для тестов под WSL2

Установить компоненты
```bash
sudo apt install alsa-plugins -y
```

Прописать чтобы воспроизводил через pulseaudio
```bash
nano ~/.asoundrc

# write
pcm.!default {
    type pulse
    fallback "sysdefault"
}

ctl.!default {
    type pulse
}
#save
```

уставить
```bash
sudo apt update && sudo apt install libasound2-plugins
```

Проверка. Это воспроизводит белый шум.
```bash
speaker-test -t pink -c 2
```
