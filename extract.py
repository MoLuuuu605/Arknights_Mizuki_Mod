#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
将webm视频逐帧提取为PNG图片的脚本
支持图片左右翻转功能
"""

import cv2
import os
import sys
from pathlib import Path

def extract_frames_from_webm(video_path, output_dir=None, prefix="frame", start_frame=0, end_frame=None, step=1, flip=False):
    """
    从webm视频中提取帧并保存为PNG图片
    
    参数:
        video_path: 视频文件路径
        output_dir: 输出目录（默认为视频同目录下的frames文件夹）
        prefix: 图片文件名前缀（默认"frame"）
        start_frame: 起始帧（默认0）
        end_frame: 结束帧（默认None表示到视频结束）
        step: 间隔多少帧提取一帧（默认1表示逐帧提取）
        flip: 是否左右翻转图片（默认False）
    """
    # 检查视频文件是否存在
    if not os.path.exists(video_path):
        print(f"错误：视频文件不存在 - {video_path}")
        return False
    
    # 创建输出目录
    if output_dir is None:
        # 默认在视频文件同目录下创建frames文件夹
        video_dir = os.path.dirname(video_path)
        video_name = os.path.splitext(os.path.basename(video_path))[0]
        output_dir = os.path.join(video_dir, f"{video_name}_frames")
    
    Path(output_dir).mkdir(parents=True, exist_ok=True)
    
    # 打开视频文件
    cap = cv2.VideoCapture(video_path)
    
    if not cap.isOpened():
        print(f"错误：无法打开视频文件 - {video_path}")
        print("请确保：")
        print("1. 文件路径正确")
        print("2. 已安装opencv-python和opencv-contrib-python")
        print("3. 视频文件没有损坏")
        return False
    
    # 获取视频信息
    total_frames = int(cap.get(cv2.CAP_PROP_FRAME_COUNT))
    fps = cap.get(cv2.CAP_PROP_FPS)
    width = int(cap.get(cv2.CAP_PROP_FRAME_WIDTH))
    height = int(cap.get(cv2.CAP_PROP_FRAME_HEIGHT))
    
    print(f"视频信息:")
    print(f"  总帧数: {total_frames}")
    print(f"  FPS: {fps:.2f}")
    print(f"  分辨率: {width}x{height}")
    print(f"  左右翻转: {'是' if flip else '否'}")
    print(f"输出目录: {output_dir}")
    print("-" * 50)
    
    # 设置结束帧
    if end_frame is None or end_frame > total_frames:
        end_frame = total_frames
    
    # 提取帧
    frame_count = 0
    saved_count = 0
    
    while True:
        ret, frame = cap.read()
        
        if not ret:
            break
        
        # 只处理指定范围的帧
        if frame_count >= start_frame and frame_count < end_frame:
            # 按步长提取
            if (frame_count - start_frame) % step == 0:
                # 如果需要翻转，进行左右翻转
                if flip:
                    frame = cv2.flip(frame, 1)  # 1 表示水平翻转（左右）
                
                # 生成文件名（使用6位数字编号）
                filename = f"{prefix}_{saved_count:06d}.png"
                filepath = os.path.join(output_dir, filename)
                
                # 保存图片
                cv2.imwrite(filepath, frame)
                saved_count += 1
                
                # 显示进度
                if saved_count % 10 == 0:
                    progress = (frame_count - start_frame + 1) / (end_frame - start_frame) * 100
                    print(f"进度: {progress:.1f}% - 已提取 {saved_count} 帧")
        
        frame_count += 1
        
        # 如果超过结束帧，退出循环
        if frame_count >= end_frame:
            break
    
    cap.release()
    
    print("-" * 50)
    print(f"完成！共提取了 {saved_count} 帧图片")
    print(f"图片保存位置: {output_dir}")
    
    return True

def main():
    """主函数：处理命令行参数"""
    import argparse
    
    parser = argparse.ArgumentParser(
        description="将webm视频逐帧提取为PNG图片（支持左右翻转）",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
使用示例:
  # 基本用法（默认输出到frames文件夹）
  python extract_frames.py video.webm
  
  # 指定输出目录
  python extract_frames.py video.webm -o ./my_frames
  
  # 指定文件名前缀
  python extract_frames.py video.webm -p img
  
  # 提取指定范围的帧（从第100帧到第500帧）
  python extract_frames.py video.webm -s 100 -e 500
  
  # 每隔5帧提取一帧
  python extract_frames.py video.webm --step 5
  
  # 左右翻转提取的图片
  python extract_frames.py video.webm --flip
  
  # 组合使用
  python extract_frames.py video.webm -o ./output -p frame -s 100 -e 1000 --step 2 --flip
        """
    )
    
    parser.add_argument("video", help="webm视频文件路径")
    parser.add_argument("-o", "--output", help="输出目录（默认：视频文件名_frames）")
    parser.add_argument("-p", "--prefix", default="frame", help="图片文件名前缀（默认：frame）")
    parser.add_argument("-s", "--start", type=int, default=0, help="起始帧索引（默认：0）")
    parser.add_argument("-e", "--end", type=int, help="结束帧索引（默认：视频总帧数）")
    parser.add_argument("--step", type=int, default=1, help="间隔多少帧提取一帧（默认：1，逐帧提取）")
    parser.add_argument("--flip", action="store_true", help="是否左右翻转提取的图片")
    
    args = parser.parse_args()
    
    # 执行提取
    success = extract_frames_from_webm(
        video_path=args.video,
        output_dir=args.output,
        prefix=args.prefix,
        start_frame=args.start,
        end_frame=args.end,
        step=args.step,
        flip=args.flip
    )
    
    sys.exit(0 if success else 1)

if __name__ == "__main__":
    main()