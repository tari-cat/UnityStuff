# AudioManager
A relatively simple yet powerful and versatile audio management tool, used for playing audio at locations with pooled audio sources <br/>
To set it up, create an empty GameObject, and add 32 child GameObjects to it. <br/>
In the parent GameObject, attach the AudioManager component to it. <br/>
In every child GameObject, attach the AudioSource component. <br/>
<br/>
To use, simply call AudioManager as you would call a static class. <br/>
```AudioManager.Play("mySound", 1f);``` will play a sound named mySound from the `Resources/Audio` folder. <br/>
