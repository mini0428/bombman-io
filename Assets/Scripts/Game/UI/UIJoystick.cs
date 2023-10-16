using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class UIJoystick : MonoBehaviour
	{
		public enum MovementAxes { XandY, X, Y };

		public SimpleInput.AxisInput xAxis = new SimpleInput.AxisInput( "Horizontal" );
		public SimpleInput.AxisInput yAxis = new SimpleInput.AxisInput( "Vertical" );

		private RectTransform joystickTR;
		private Graphic background;

		public MovementAxes movementAxes = MovementAxes.XandY;
		public float valueMultiplier = 1f;

#pragma warning disable 0649
		[SerializeField]
		private Image thumb;
		private RectTransform thumbTR;

		[SerializeField]
		private float movementAreaRadius = 75f;

		[Tooltip( "Radius of the deadzone at the center of the joystick that will yield no input" )]
		[SerializeField]
		private float deadzoneRadius;

		[SerializeField]
		private bool isDynamicJoystick = false;

		[SerializeField]
		private RectTransform dynamicJoystickMovementArea;

		[SerializeField]
		private bool canFollowPointer = false;
#pragma warning restore 0649

		private bool joystickHeld = false;
		private Vector2 pointerInitialPos;

		private float _1OverMovementAreaRadius;
		private float movementAreaRadiusSqr;
		private float deadzoneRadiusSqr;

		private Vector2 joystickInitialPos;

		private float opacity = 1f;

		private Vector2 m_value = Vector2.zero;
		public Vector2 Value { get { return m_value; } }

		private void Awake()
		{
			joystickTR = (RectTransform) transform;
			thumbTR = thumb.rectTransform;
			background = GetComponent<Graphic>();

			if( isDynamicJoystick )
			{
				opacity = 0f;
				thumb.raycastTarget = false;
				if( background )
					background.raycastTarget = false;

				Update();
			}
			else
			{
				thumb.raycastTarget = true;
				if( background )
					background.raycastTarget = true;
			}

			_1OverMovementAreaRadius = 1f / movementAreaRadius;
			movementAreaRadiusSqr = movementAreaRadius * movementAreaRadius;
			deadzoneRadiusSqr = deadzoneRadius * deadzoneRadius;

			joystickInitialPos = joystickTR.anchoredPosition;
			thumbTR.localPosition = Vector3.zero;
		}

		private void OnEnable()
		{
			xAxis.StartTracking();
			yAxis.StartTracking();
		}

		private void OnDisable()
		{
			OnPointerUp();

			xAxis.StopTracking();
			yAxis.StopTracking();
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			_1OverMovementAreaRadius = 1f / movementAreaRadius;
			movementAreaRadiusSqr = movementAreaRadius * movementAreaRadius;
			deadzoneRadiusSqr = deadzoneRadius * deadzoneRadius;
		}
#endif

		public void OnPointerDown( Vector2 pos )
		{
			joystickHeld = true;

			if( isDynamicJoystick )
			{
				pointerInitialPos = Vector2.zero;

				Vector3 joystickPos;
				RectTransformUtility.ScreenPointToWorldPointInRectangle( dynamicJoystickMovementArea, pos, UICommon.UiCam, out joystickPos );
				joystickTR.position = joystickPos;
			}
			else
				RectTransformUtility.ScreenPointToLocalPointInRectangle( joystickTR, pos, UICommon.UiCam, out pointerInitialPos );
		}

		public void OnDrag( Vector2 pos )
		{
			Vector2 pointerPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle( joystickTR, pos, UICommon.UiCam, out pointerPos );

			Vector2 direction = pointerPos - pointerInitialPos;
			if( movementAxes == MovementAxes.X )
				direction.y = 0f;
			else if( movementAxes == MovementAxes.Y )
				direction.x = 0f;

			if( direction.sqrMagnitude <= deadzoneRadiusSqr )
				m_value.Set( 0f, 0f );
			else
			{
				if( direction.sqrMagnitude > movementAreaRadiusSqr )
				{
					Vector2 directionNormalized = direction.normalized * movementAreaRadius;
					if( canFollowPointer )
						joystickTR.localPosition += (Vector3) ( direction - directionNormalized );

					direction = directionNormalized;
				}

				m_value = direction * _1OverMovementAreaRadius * valueMultiplier;
			}

			thumbTR.localPosition = direction;

			xAxis.value = m_value.x;
			yAxis.value = m_value.y;
		}

		public void OnPointerUp()
		{
			joystickHeld = false;
			m_value = Vector2.zero;

			thumbTR.localPosition = Vector3.zero;
			if( !isDynamicJoystick && canFollowPointer )
				joystickTR.anchoredPosition = joystickInitialPos;

			xAxis.value = 0f;
			yAxis.value = 0f;
			
			isTouchScreen = false;
		}

		private void Update()
		{
			if( !isDynamicJoystick )
				return;

			UpdateMouse();

			if( joystickHeld )
				opacity = Mathf.Min( 1f, opacity + Time.unscaledDeltaTime * 4f );
			else
				opacity = Mathf.Max( 0f, opacity - Time.unscaledDeltaTime * 4f );

			Color c = thumb.color;
			c.a = opacity;
			thumb.color = c;

			if( background )
			{
				c = background.color;
				c.a = opacity;
				background.color = c;
			}
		}

		private bool isTouchScreen = false;
		private Vector2 touchPosition;
		private void UpdateMouse()
		{
			if (Input.GetMouseButton(0))
			{
				var mousePos = Input.mousePosition;
				if (!isTouchScreen)
				{
					touchPosition = mousePos;
					OnPointerDown(mousePos);
				}
				
				isTouchScreen = true;

				if (!mousePos.Equals(touchPosition))
				{
					OnDrag(mousePos);
				}
			}
			else
			{
				if (isTouchScreen)
				{
					OnPointerUp();
				}
				isTouchScreen = false;
			}
		}
	}
}
