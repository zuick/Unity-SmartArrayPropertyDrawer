using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer( typeof( SmartArrayAttribute ) )]
public class SmartArrayPropertyDrawer : PropertyDrawer {
	
	public class PropertyReference
	{
		public Object target;
		public string path;
		public int index;
	}
	
	const float c_fieldHeight = 16.0f;
	const float c_helpboxHeight = 24.0f;
	
	bool heightIsChanged = true;
	float cacheHeight = 0.0f;
	
	/// <summary>
	/// Draw Property.
	/// </summary>
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginChangeCheck();
		
		if( property.isArray )
		{
			// cache for Right-Click-Menu's callback.
			string targetPropertyPath = property.propertyPath;
			Object targetObject = property.serializedObject.targetObject;
			
			// draw header.
			DrawArrayExpander( ref position, property, label );
			
			if( property.isExpanded )
			{
				// want open menu?.
				bool openMenu = WantOpenMenu();
				Vector3 mousePos = Event.current.mousePosition;
								
				// draw array size.
				EditorGUI.indentLevel++;
				DrawArraySize( ref position, property, targetObject, targetPropertyPath, openMenu, mousePos );
				
				// draw array elements.
				int size = property.arraySize;
				for( int i=0; i<size; ++i )
				{
					SerializedProperty propElement = property.GetArrayElementAtIndex( i );
					SerializedProperty propEnd     = propElement.GetEndProperty();
					
					bool isFirst = true;
					
					// draw array elements internal.
					do
					{
						position.height = EditorGUI.GetPropertyHeight( propElement, label, false );
						
						Vector2 pos = EditorGUIUtility.GUIToScreenPoint( new Vector3( position.xMin, position.yMin ) );
						
						// draw only inside of screen.
						if( 0 < pos.y && pos.y < Screen.currentResolution.height + position.height )
						{
							EditorGUI.PropertyField( position, propElement );
						}

						// Right-Click-Menu collision on element header only.
						if( isFirst )
						{
							if( openMenu && position.Contains( mousePos ) )
							{ OpenArrayElementMenu( targetObject, targetPropertyPath, i, mousePos ); }
							isFirst = false;
						}
						
						position.y += position.height;
					}
					while( propElement.NextVisible( propElement.isExpanded ) &&
							!SerializedProperty.EqualContents( propElement, propEnd ) );
				}
				
				EditorGUI.indentLevel--;
			}
		}
		else
		{
			EditorGUI.BeginProperty( position, label, property );
			EditorGUI.HelpBox( position, "[ERROR:SmartArray] '" + label.text + "' field is not Array or List.", MessageType.Error );
			position.y += c_helpboxHeight;
			EditorGUI.EndProperty();
		}
		
		if( EditorGUI.EndChangeCheck() )
		{
			heightIsChanged = true;
		}
	}
	
	/// <summary>
	/// Gets the height of the property.
	/// </summary>
	public override float GetPropertyHeight ( SerializedProperty property, GUIContent label )
	{
		float h = 0.0f;
		
		if( !heightIsChanged )
		{
			return cacheHeight;
		}
		
		if( property.isArray )
		{
			// header.
			h += c_fieldHeight;
			
			if( property.isExpanded )
			{
				// size.
				{
					SerializedProperty propCopy = property.Copy();
					propCopy.Next( true );
					h += EditorGUI.GetPropertyHeight( propCopy );
				}
				
				// elements.
				int size = property.arraySize;
				for( int i=0; i<size; ++i )
				{
					SerializedProperty propElement = property.GetArrayElementAtIndex( i );
					SerializedProperty propEnd     = propElement.GetEndProperty();
					
					// elements internal.
					do
					{ h += EditorGUI.GetPropertyHeight( propElement, label, false ); }
					while( propElement.NextVisible( propElement.isExpanded ) &&
							!SerializedProperty.EqualContents( propElement, propEnd ) );
				}
			}
		}
		else
		{	// helpbox.
			h += c_helpboxHeight;
		}
		
		cacheHeight = h;
		heightIsChanged = false;
		
		return h;
	}
	
	/// <summary>
	/// Draws the array expander.
	/// </summary>
	void DrawArrayExpander( ref Rect position, SerializedProperty property, GUIContent label )
	{
		EditorGUI.indentLevel--;
		position.height = c_fieldHeight;
		property.isExpanded = EditorGUI.Foldout( position, property.isExpanded, label );
		position.y += c_fieldHeight;
		EditorGUI.indentLevel++;
	}
	
	/// <summary>
	/// Draws the size of the array.
	/// </summary>
	void DrawArraySize( ref Rect position, SerializedProperty property, Object targetObject, string targetPropertyPath, bool openMenu, Vector3 mousePos )
	{
		property = property.Copy();
		property.NextVisible( true );
		EditorGUI.PropertyField( position, property );
		
		// Right-Click-Menu.
		if( openMenu && position.Contains( mousePos ) )
		{ OpenArraySizeFieldMenu( targetObject, targetPropertyPath, mousePos ); }
		
		position.y += EditorGUI.GetPropertyHeight( property );
	}
	
	/// <summary>
	/// Wants the open menu?.
	/// </summary>
	bool WantOpenMenu()
	{
		return 
			Event.current.type == EventType.ContextClick &&		// click.
			Event.current.button == 1;							// mouse right button.
	}
	
	/// <summary>
	/// Opens the array size field menu.
	/// </summary>
	void OpenArraySizeFieldMenu( Object targetObject, string targetPropertyPath, Vector3 mousePos )
	{
		PropertyReference propRef = new PropertyReference();
		propRef.target = targetObject;
		propRef.path   = targetPropertyPath;
		Rect menuRect = new Rect( mousePos.x, mousePos.y, 0, 0 );
		EditorUtility.DisplayCustomMenu( menuRect, CreateArraySizeMenuContents(), -1, SelectArraySizeMenuCallback, propRef );
	}
	
	/// <summary>
	/// Opens the array element menu.
	/// </summary>
	void OpenArrayElementMenu( Object targetObject, string targetPropertyPath, int index, Vector3 mousePos )
	{
		PropertyReference propRef = new PropertyReference();
		propRef.target = targetObject;
		propRef.path   = targetPropertyPath;
		propRef.index   = index;
		Rect menuRect = new Rect( mousePos.x, mousePos.y, 0, 0 );
		EditorUtility.DisplayCustomMenu( menuRect, CreateArrayElementMenuContents( index ), -1, SelectArrayElementMenuCallback, propRef );
	}
	
	/// <summary>
	/// Creates the array size menu contents.
	/// </summary>
	GUIContent[] CreateArraySizeMenuContents()
	{	
		GUIContent[] retval = new GUIContent[]{ 
			new GUIContent( "Add/First" ),
			new GUIContent( "Add/Last" ),
			new GUIContent( "Remove/First" ),
			new GUIContent( "Remove/Last" ),
		};
			
		return retval;
	}
	
	/// <summary>
	/// Selects the array size menu's Callback.
	/// </summary>
	void SelectArraySizeMenuCallback( object userData, string[] options, int selected )
	{
		PropertyReference propRef = (PropertyReference)userData;
		var so = new SerializedObject( propRef.target );
		var prop = so.FindProperty( propRef.path );

		switch( selected )
		{
		case 0:	// Add/First
			prop.InsertArrayElementAtIndex( 0 );
			so.ApplyModifiedProperties();
			heightIsChanged = true;
			break;
			
		case 1:	// Add/Last
			prop.InsertArrayElementAtIndex( Mathf.Max( 0, prop.arraySize - 1 ) );
			so.ApplyModifiedProperties();
			heightIsChanged = true;
			break;
			
		case 2:	// Remove/First
			if( prop.arraySize > 0 )
			{
				prop.DeleteArrayElementAtIndex( 0 );
				so.ApplyModifiedProperties();
				heightIsChanged = true;
			}
			break;
			
		case 3:	// Remove/Last.
			if( prop.arraySize > 0 )
			{
				prop.DeleteArrayElementAtIndex( Mathf.Max( 0, prop.arraySize - 1 ) );
				so.ApplyModifiedProperties();
				heightIsChanged = true;
			}
			break;
			
		default:
			Debug.Log( "Undefined menu item is selected = " + options[ selected ] );
			break;
		}
	}
	
	/// <summary>
	/// Creates the array element menu contents.
	/// </summary>
	GUIContent[] CreateArrayElementMenuContents( int index )
	{
		GUIContent[] retval = new GUIContent[]
		{
			new GUIContent( "Add/Before \'Element " + index + "\'" ),
			new GUIContent( "Add/After \'Element " + index + "\'" ),
			new GUIContent( "Remove/\'Element " + index + "\'" ),
		};
		
		return retval;
	}
	
	/// <summary>
	/// Selects the array element menu's Callback.
	/// </summary>
	void SelectArrayElementMenuCallback( object userData, string[] options, int selected )
	{
		PropertyReference propRef = (PropertyReference)userData;
		var so = new SerializedObject( propRef.target );
		var prop = so.FindProperty( propRef.path );
		var i = propRef.index;
		
		switch( selected )
		{
		case 0: // Add/Before Element X.
			prop.InsertArrayElementAtIndex( i );
			so.ApplyModifiedProperties();
			heightIsChanged = true;
			break;
		case 1: // Add/After Element X.
			prop.InsertArrayElementAtIndex( i + 1 );
			so.ApplyModifiedProperties();
			heightIsChanged = true;
			break;
		case 2: // Delete/Element X.
			if( prop.arraySize > 0 )
			{
				prop.DeleteArrayElementAtIndex( Mathf.Max( 0, i ) );
				so.ApplyModifiedProperties();
				heightIsChanged = true;
			}
			break;
			
		default:
			Debug.Log( "Undefined menu item is selected = " + options[ selected ] );
			break;
		}
	}
}
