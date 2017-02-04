# AndroidSlideLayout
[![NuGet version](https://badge.fury.io/nu/Meilcli.Android.SlideLayout.svg)](https://www.nuget.org/packages/Meilcli.Android.SlideLayout/)  
SlideLayout for Xamarin.Android

SlideLayout' children view can drag!!

Allow Directions All, Vertical, Horizontal, Top, Bottom, Left, Right.  
![](/all_vertical_horizontal.gif)
![](/top_bottom_left_right.gif)

## Required
- MonoAndroid7.0 (if older version? should update Xamarin.Android)

## Install
~~~
Install-Package Meilcli.Android.SlideLayout
~~~

## Usage
How use in your Project? see [sample AndroidSlideLayout.App](/AndroidSlideLayout.App)!!!

### How add to layout.xml?
First, add this code to root layout in xml
 
```xml
xmlns:app="http://schemas.android.com/apk/res-auto"
```

Second, add SlideLayout and child view in xml

```xml
<androidslidelayout.SlideLayout
  android:layout_width="match_parent"
  android:layout_height="match_parent">
  <View
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    app:draggable_all_direction="true" />
</androidslidelayout.SlideLayout>
```

### How customize directions?
Set draggable_*_direction in SlideLayout's child view
```
app:draggable_all_direction="true"
app:draggable_vertical_direction="true"
app:draggable_horizontal_direction="true"
app:draggable_top_direction="true"
app:draggable_bottom_direction="true"
app:draggable_left_direction="true"
app:draggable_right_direction="true"
```

### Activity Transition and Back Motion
![](/transition.gif)  

See sample
- [TransitionActivity](/AndroidSlideLayout.App/TransitionActivity.cs)
- [Transition.axml](/AndroidSlideLayout.App/Resources/layout/Transition.axml)
- [Styles.xml (for lolipop)](/AndroidSlideLayout.App/Resources/values-v21/Styles.xml)

## License
This library is under MIT License.

### Thanks
This library use [Xamarin.Android.Support Library](https://github.com/xamarin/AndroidSupportComponents/)  
Xamarin.Android.Support is under [MIT License](https://github.com/xamarin/AndroidSupportComponents/blob/master/LICENSE.md)  
Copyright (c) .NET Foundation Contributors  

This library use code that ported from FrameLayout to Xamarin  
FrameLayout is under Apache License v2  
Copyright (C) 2006 The Android Open Source Project

