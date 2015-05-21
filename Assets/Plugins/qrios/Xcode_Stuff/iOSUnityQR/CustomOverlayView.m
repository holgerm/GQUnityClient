//
//  CustomOverlayView.m
//  Unity-iPhone
//
//  Created by Xiang Wang on 07.06.13.
//
//

#import "CustomOverlayView.h"

@implementation CustomOverlayView

- (id)initWithFrame:(CGRect)frame
{
    self = [super initWithFrame:frame];
    if (self) {
        // Initialization code
    }
    return self;
}

- (void)willMoveToSuperview:(UIView *)newSuperview {
    UILabel* testLabel = [[UILabel alloc] initWithFrame:CGRectMake(0, 0, 150, 30)];
    testLabel.text = @" ";
    
    [self addSubview:testLabel];
}

/*
// Only override drawRect: if you perform custom drawing.
// An empty implementation adversely affects performance during animation.
- (void)drawRect:(CGRect)rect
{
    // Drawing code
}
*/

@end
