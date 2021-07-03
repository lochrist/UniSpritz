using UnityEngine;
using System.Collections;

public class PixelPerfectCamera : MonoBehaviour
{
	public static int gameFps = 50; //Frames por segundo del juego.
	public static float texturesSize = 100f; //Es el valor de la propiedad 'Pixels to Units' de las imagenes que cargo en Unity.
	public static int myWidthResolution = 320;
	public static int myHeightResolution = 200;
	public int GameFps;
	public int virtualWidthResolution; //Variable para mostrar el valor en el 'Inspector' de Unity.
	public int virtualHeightResolution; //Variable para mostrar el valor en el 'Inspector' de Unity.
	public float virtualAspectRatio; //Variable para mostrar el valor en el 'Inspector' de Unity.
	public float orthograpicCameraSize;

	public static float unitsPerPixel;

	public PixelPerfectCamera()
	{
		virtualWidthResolution = myWidthResolution;
		virtualHeightResolution = myHeightResolution;

		unitsPerPixel = 1f / texturesSize;
	}

	void Awake()
	{
		//FrameRate (Frames por segundo del juego):
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = gameFps;
		GameFps = gameFps;
	}

	// Use this for initialization
	void Start()
	{
		virtualWidthResolution = myWidthResolution;
		virtualHeightResolution = myHeightResolution;

		//El tamaño de la camara ortografica es el mismo que el del FrameBuffer donde se dibuja la escena y luego
		//este se dibuja en memoria de video (pantalla). En vez de cambiar la resolucion del dispositivo destino,
		//Unity lo que se hace es "escalar" el frameBuffer (altoxancho o resolucion) a la resolucion actual del dispositivo.
		//Asi la imagen del juego siempre ocupara toda la pantalla, el problema es que dependiendo de la relacion de aspecto
		//(ancho/alto) de la resolucion destino y la resolucion con la que se programa el juego, si hay mucha diferencia entre
		//ambas, la imagen final en pantalla puede verse distorsionada respecto al ancho y alto. Para solucionar esto se
		//utliza el script 'AspectRatio' que cambia el tamaño del 'ViewPort' (region de pantalla sobre la que se va a copiar o
		//visualizar el frameBuffer (camera) añadiendo lineas negras a los lados o arriba abajo para simular que el dispositivo
		//destino tiene una resolucion con un aspect ratio igual al de la resolucion con la que se programo el juego (tamaño de camara).
		//Asi al escalar la imagen del frameBuffer se evita la deformacion, ya que la relacion entre el ancho y el alto de ambas
		//resoluciones es la misma, solo varian sus valores.


		//Se establece el tamaño de la camara ortografica (alto) y su ancho (aspectRatio) para que coincida con el nº de pixeles
		//de la resolucion con la que se programa el juego (resolucion virtual).
		//El escalado de la resolucion virtual (backBuffer) a la resolucion actual de la pantalla se hace de forma automatica
		//por Unity al ejecutar el juego. En Escritorio se escala al tamaño total de la resolucion actual de la pantalla, en Windows 8
		//se escala manteniendo la proporcion de aspectRatio entre la resolucion virtual y la de la pantalla (cambia automaticamente 
		//el tamaño del viewport en pantalla).
		Camera.main.orthographicSize = (myHeightResolution / 2f) * unitsPerPixel;
		Camera.main.aspect = (float)myWidthResolution / myHeightResolution;

		orthograpicCameraSize = Camera.main.orthographicSize;
		virtualAspectRatio = Camera.main.aspect;
	}

	void Update()
	{

	}

	/// <summary>
	/// Convierte una posicion 2D segun la 'resolucion virtual' del juego en una posicion de la 'Camara' o 'World'.
	/// </summary>
	/// <returns>Posicion de Camara o Mundo.</returns>
	/// <param name="position2D">Posicion 2D segun resolucion virtual.</param>
	public static Vector3 Position2DToWorld(Vector3 position2D)
	{
		//Las posiciones 2D de la 'resolucion virtual' del juego tienen el punto (0,0) en el centro de la pantalla y son
		//enteras, cada coordenada de apunta a un punto/pixel de pantalla. Son las coordenadas comunes a todos los objetos
		//del juego para definir su posicion en la 'resolucion virtual' en la que se programa el juego, y asi después el motor
		//Unity escala toda la imagen renderizada en cada frame de juego (FrameBuffer o BackBuffer) a las dimensiones de la
		//'resolución real' de la pantalla del dispositivo donde se está ejecutando el juego.
		position2D.x *= unitsPerPixel;
		position2D.y *= unitsPerPixel;

		return position2D;
	}

	/// <summary>
	/// Convierte una posicion del 'Mundo' o 'Camara' en una posicion 2D de la 'resolucion virtual' del juego.
	/// </summary>
	/// <returns>Posicion 2D segun resolucion virtual.</returns>
	/// <param name="positionWorld">Posicion de Camara o Mundo.</param>
	public static Vector3 WorldTo2DPosition(Vector3 positionWorld)
	{
		//Las posiciones del 'Mundo' o 'Camara' tiene el punto (0,0) en el centro de pantalla pero son decimales.
		//Su valor varia en funcion del valor 'Pixels to Units' de cada Imagen cargada y del tamaño (size) de la 'Camara'
		//de la Escena activa.
		//Las utilizan los 'GameObjects' y 'Sprites' en su componente 'transform' para indicar su posicion en pantalla.
		positionWorld.x *= texturesSize;
		positionWorld.y *= texturesSize;

		/*Es lo mismo:
		positionWorld.x /= unitsPerPixel;
		positionWorld.y /= unitsPerPixel;*/

		return positionWorld;
	}

	/// <summary>
	/// Convierte una posicion de 'Pantalla' (raton) en una posicion 2D de la 'resolucion virtual' del juego.
	/// </summary>
	/// <returns>Posicion 2D segun resolucion virtual.</returns>
	/// <param name="positionScreen">Posicion de Pantalla.</param>
	public static Vector3 ScreenTo2DPosition(Vector3 positionScreen)
	{
		//Las posiciones de 'Pantalla' tienen el punto (0,0) en la esquina inferior-izquierda de la pantalla.
		//Son enteras y utilizan la resolucion actual de la pantalla (no la virtual del juego).
		//Las utiliza el raton y la pantalla táctil (Input.mousePosition, Input.Touch) para indicar la posicion en pantalla.

		positionScreen = Camera.main.ScreenToWorldPoint(positionScreen);
		positionScreen = WorldTo2DPosition(positionScreen);

		return positionScreen;
	}

	/// <summary>
	/// Convierte una posicion 2D de la 'resolucion virtual' del juego en una posicion de 'Pantalla' (raton).
	/// </summary>
	/// <returns>Posicion de Pantalla.</returns>
	/// <param name="position2D">Posicion 2D segun resolucion virtual.</param>
	public static Vector3 Position2DToScreen(Vector3 position2D)
	{
		position2D = Position2DToWorld(position2D);
		position2D = Camera.main.WorldToScreenPoint(position2D);

		return position2D;
	}

	/// <summary>
	/// Este metodo convierte una posicion 2D de la 'resolucion virtual' del juego en una posicion de 'ViewPort'
	/// (GuiText, GuiTexture).
	/// </summary>
	/// <returns>The D to view port.</returns>
	/// <param name="position2D">Position2 d.</param>
	public static Vector3 Position2DToViewPort(Vector3 position2D)
	{
		//Las coordenadas ViewPort tienen el punto (0,0) en la esquina inferior izquierda de la pantalla y van de 0 a 1 porque
		//son relativas al tamaño del ViewPort de la Camara (Camera.Rect).
		//Las utilizan los gameObjects 'GuiText' y 'GuiTexture' en su componente 'transform' para indicar la posicion en pantalla.
		//A partir de la versión 4.6 de Unity se cambió la gestión de UI y los objetos GuiText y GuiTexture ya no se pueden
		//añadir desde el inspector de Unity (pero si por código) y se crearon los contenedores (Canvas...) para posicionar
		//los nuevos elementos de interfaz de usuario.

		float xViewPort, yViewPort;
		xViewPort = (float)position2D.x / PixelPerfectCamera.myWidthResolution;
		yViewPort = (float)position2D.y / PixelPerfectCamera.myHeightResolution;

		//Para que el punto (0,0) de 2D esté en el centro de la pantalla en coordenadas ViewPort:
		xViewPort += 0.5f;
		yViewPort += 0.5f;

		Vector3 posicionViewPort = new Vector3(xViewPort, yViewPort, position2D.z);
		return posicionViewPort;
	}

	public static Vector3 ViewPortTo2DPosition(Vector3 positionViewPort)
	{
		int x2D, y2D;

		x2D = (int)positionViewPort.x * PixelPerfectCamera.myWidthResolution;
		y2D = (int)positionViewPort.y * PixelPerfectCamera.myHeightResolution;

		//Para que la posicion 2D tenga el punto (0,0) en el centro de la pantalla:
		x2D -= PixelPerfectCamera.myWidthResolution / 2;
		y2D -= PixelPerfectCamera.myHeightResolution / 2;

		Vector3 position2D = new Vector3(x2D, y2D, positionViewPort.z);
		return position2D;
	}

	public static Vector3 Position2DToGUIRect(Vector3 position2D)
	{
		//Cuando se utilizan los métodos estáticos de la clase 'GUI' para dibujar controles de interfaz de usuario (Texture2D, Label...)
		//la posición se define mediante una estructura de tipo 'Rect', que toman como punto (0,0) la esquina superior-izquierda
		//de la pantalla y son siempre enteros positivos (2D normal de toda la vida). El problema es que este sistema de coordenadas
		//funciona sobre la resolución real del dispositivo sobre el que se ejecuta el juego, y no tiene en cuenta el tamaño del ViewPort
		//de la cámara. Para ello hay que realizar unos cálculos adicionales.

		Vector3 guiRectPosition = new Vector3();
		Vector3 ptoSupIzq = new Vector3(-(PixelPerfectCamera.myWidthResolution / 2), (PixelPerfectCamera.myHeightResolution / 2), position2D.z);

		//Cambio de eje de coordenadas.
		guiRectPosition.x = position2D.x - ptoSupIzq.x;
		guiRectPosition.y = ptoSupIzq.y - position2D.y;
		guiRectPosition.z = position2D.z;

		//Teniendo en cuenta la relación entre la resolución real y la virtual.
		guiRectPosition.x = guiRectPosition.x * ((float)Screen.width / PixelPerfectCamera.myWidthResolution);
		guiRectPosition.y = guiRectPosition.y * ((float)Screen.height / PixelPerfectCamera.myHeightResolution);

		//Teniendo en cuenta el tamaño del ViewPort de la cámara (AspectRatio). Se suman a cada coordenada el ancho/alto de los posibles
		//rectángulos negros que se usan para limitar el ViewPort en pantalla.
		guiRectPosition.x = guiRectPosition.x + (Camera.main.rect.x * Screen.width);
		guiRectPosition.y = guiRectPosition.y + (Camera.main.rect.y * Screen.height);

		return guiRectPosition;
	}

	public static Vector3 GUIRectTo2DPosition(Vector3 guiRectPosition)
	{
		Vector3 pos2D = new Vector3();
		Vector2 ptoCentroGuiRect = new Vector2((PixelPerfectCamera.myWidthResolution / 2), (PixelPerfectCamera.myHeightResolution / 2));

		//Teniendo en cuenta el ViewPort de la Camara (Aspect Ratio).
		//Nota.- Hay que realizar antes el cálculo para el ViewPort en vez del cambio de resolución porque en Unity el tamaño
		//de los rectángulos que limitan el aspectRatio de la Pantalla miden su tamaño en puntos de la 'resolución real', no de
		//la 'resolución virtual' del juego.
		guiRectPosition.x = guiRectPosition.x - (Camera.main.rect.x * Screen.width);
		guiRectPosition.y = guiRectPosition.y - (Camera.main.rect.y * Screen.height);

		//Teniendo en cuenta la relación entre la resolución real y la virtual.
		guiRectPosition.x = guiRectPosition.x * ((float)PixelPerfectCamera.myWidthResolution / Screen.width);
		guiRectPosition.y = guiRectPosition.y * ((float)PixelPerfectCamera.myHeightResolution / Screen.height);

		//Cambio de eje de coordenadas.
		pos2D.x = guiRectPosition.x - ptoCentroGuiRect.x;
		pos2D.y = ptoCentroGuiRect.y - guiRectPosition.y;
		pos2D.z = guiRectPosition.z;

		return pos2D;
	}

}