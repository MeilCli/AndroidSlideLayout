
var camelCaseTokenizer = function (obj) {
    var previous = '';
    return obj.toString().trim().split(/[\s\-]+|(?=[A-Z])/).reduce(function(acc, cur) {
        var current = cur.toLowerCase();
        if(acc.length === 0) {
            previous = current;
            return acc.concat(current);
        }
        previous = previous.concat(current);
        return acc.concat([current, previous]);
    }, []);
}
lunr.tokenizer.registerFunction(camelCaseTokenizer, 'camelCaseTokenizer')
var searchModule = function() {
    var idMap = [];
    function y(e) { 
        idMap.push(e); 
    }
    var idx = lunr(function() {
        this.field('title', { boost: 10 });
        this.field('content');
        this.field('description', { boost: 5 });
        this.field('tags', { boost: 50 });
        this.ref('id');
        this.tokenizer(camelCaseTokenizer);

        this.pipeline.remove(lunr.stopWordFilter);
        this.pipeline.remove(lunr.stemmer);
    });
    function a(e) { 
        idx.add(e); 
    }

    a({
        id:0,
        title:"SlideLayout LayoutParams",
        content:"SlideLayout LayoutParams",
        description:'',
        tags:''
    });

    a({
        id:1,
        title:"ViewDragHelperCallback",
        content:"ViewDragHelperCallback",
        description:'',
        tags:''
    });

    a({
        id:2,
        title:"Resource Styleable",
        content:"Resource Styleable",
        description:'',
        tags:''
    });

    a({
        id:3,
        title:"ViewCapturedEventArgs",
        content:"ViewCapturedEventArgs",
        description:'',
        tags:''
    });

    a({
        id:4,
        title:"Resource String",
        content:"Resource String",
        description:'',
        tags:''
    });

    a({
        id:5,
        title:"ViewPositionChangedEventArgs",
        content:"ViewPositionChangedEventArgs",
        description:'',
        tags:''
    });

    a({
        id:6,
        title:"SlideLayout",
        content:"SlideLayout",
        description:'',
        tags:''
    });

    a({
        id:7,
        title:"ViewReleasedEventArgs",
        content:"ViewReleasedEventArgs",
        description:'',
        tags:''
    });

    a({
        id:8,
        title:"Resource",
        content:"Resource",
        description:'',
        tags:''
    });

    a({
        id:9,
        title:"FrameLayoutCompat",
        content:"FrameLayoutCompat",
        description:'',
        tags:''
    });

    a({
        id:10,
        title:"Resource Attribute",
        content:"Resource Attribute",
        description:'',
        tags:''
    });

    a({
        id:11,
        title:"ViewDragStateChangedEventArgs",
        content:"ViewDragStateChangedEventArgs",
        description:'',
        tags:''
    });

    a({
        id:12,
        title:"IDragCallback",
        content:"IDragCallback",
        description:'',
        tags:''
    });

    a({
        id:13,
        title:"FrameLayoutCompat LayoutParams",
        content:"FrameLayoutCompat LayoutParams",
        description:'',
        tags:''
    });

    y({
        url:'/AndroidSlideLayout/AndroidSlideLayout/api/AndroidSlideLayout/LayoutParams',
        title:"SlideLayout.LayoutParams",
        description:""
    });

    y({
        url:'/AndroidSlideLayout/AndroidSlideLayout/api/AndroidSlideLayout/ViewDragHelperCallback',
        title:"ViewDragHelperCallback",
        description:""
    });

    y({
        url:'/AndroidSlideLayout/AndroidSlideLayout/api/AndroidSlideLayout/Styleable',
        title:"Resource.Styleable",
        description:""
    });

    y({
        url:'/AndroidSlideLayout/AndroidSlideLayout/api/AndroidSlideLayout/ViewCapturedEventArgs',
        title:"ViewCapturedEventArgs",
        description:""
    });

    y({
        url:'/AndroidSlideLayout/AndroidSlideLayout/api/AndroidSlideLayout/String',
        title:"Resource.String",
        description:""
    });

    y({
        url:'/AndroidSlideLayout/AndroidSlideLayout/api/AndroidSlideLayout/ViewPositionChangedEventArgs',
        title:"ViewPositionChangedEventArgs",
        description:""
    });

    y({
        url:'/AndroidSlideLayout/AndroidSlideLayout/api/AndroidSlideLayout/SlideLayout',
        title:"SlideLayout",
        description:""
    });

    y({
        url:'/AndroidSlideLayout/AndroidSlideLayout/api/AndroidSlideLayout/ViewReleasedEventArgs',
        title:"ViewReleasedEventArgs",
        description:""
    });

    y({
        url:'/AndroidSlideLayout/AndroidSlideLayout/api/AndroidSlideLayout/Resource',
        title:"Resource",
        description:""
    });

    y({
        url:'/AndroidSlideLayout/AndroidSlideLayout/api/AndroidSlideLayout/FrameLayoutCompat',
        title:"FrameLayoutCompat",
        description:""
    });

    y({
        url:'/AndroidSlideLayout/AndroidSlideLayout/api/AndroidSlideLayout/Attribute',
        title:"Resource.Attribute",
        description:""
    });

    y({
        url:'/AndroidSlideLayout/AndroidSlideLayout/api/AndroidSlideLayout/ViewDragStateChangedEventArgs',
        title:"ViewDragStateChangedEventArgs",
        description:""
    });

    y({
        url:'/AndroidSlideLayout/AndroidSlideLayout/api/AndroidSlideLayout/IDragCallback',
        title:"IDragCallback",
        description:""
    });

    y({
        url:'/AndroidSlideLayout/AndroidSlideLayout/api/AndroidSlideLayout/LayoutParams',
        title:"FrameLayoutCompat.LayoutParams",
        description:""
    });

    return {
        search: function(q) {
            return idx.search(q).map(function(i) {
                return idMap[i.ref];
            });
        }
    };
}();
