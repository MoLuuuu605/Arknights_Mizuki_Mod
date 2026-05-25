#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
去除黑色背景 - 无黑边版本
使用多种技术确保边缘干净
"""

import cv2
import numpy as np
import os
from pathlib import Path
import argparse
from scipy.ndimage import binary_erosion, binary_dilation
from skimage import filters, morphology

def remove_black_background_clean(image_path, output_path, edge_smooth=True, 
                                   feather_radius=2, bg_threshold=25):
    """
    干净地去除非主体黑色背景（无黑边）
    
    参数:
        image_path: 输入图片路径
        output_path: 输出图片路径
        edge_smooth: 是否平滑边缘
        feather_radius: 边缘羽化半径（像素）
        bg_threshold: 背景黑色阈值（0-255）
    """
    # 读取图片
    img = cv2.imread(image_path, cv2.IMREAD_UNCHANGED)
    if img is None:
        print(f"无法读取图片: {image_path}")
        return False
    
    # 转换为RGBA
    if len(img.shape) == 3 and img.shape[2] == 3:
        img = cv2.cvtColor(img, cv2.COLOR_BGR2BGRA)
    elif len(img.shape) == 2:
        img = cv2.cvtColor(img, cv2.COLOR_GRAY2BGRA)
    
    # 方法1：基于亮度的自适应阈值（最稳定）
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    
    # 使用Otsu自动找最佳阈值
    _, binary = cv2.threshold(gray, 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)
    
    # 形态学操作：去除小噪点，填充小空洞
    kernel = np.ones((3,3), np.uint8)
    binary = cv2.morphologyEx(binary, cv2.MORPH_CLOSE, kernel, iterations=2)
    binary = cv2.morphologyEx(binary, cv2.MORPH_OPEN, kernel, iterations=1)
    
    # 找到最大的连通区域（主体）
    num_labels, labels, stats, _ = cv2.connectedComponentsWithStats(binary, connectivity=8)
    
    if num_labels > 1:
        # 找出面积最大的区域（排除背景）
        areas = stats[1:, cv2.CC_STAT_AREA]
        max_area_idx = np.argmax(areas) + 1
        binary = (labels == max_area_idx).astype(np.uint8) * 255
    
    # 膨胀主体掩码，确保边缘被包含
    dilate_kernel = np.ones((feather_radius * 2 + 1, feather_radius * 2 + 1), np.uint8)
    binary_dilated = cv2.dilate(binary, dilate_kernel, iterations=1)
    
    # 创建alpha通道（0=透明，255=不透明）
    alpha = binary_dilated
    
    if edge_smooth:
        # 边缘羽化：使用高斯模糊让边缘渐变
        alpha_float = alpha.astype(np.float32) / 255.0
        alpha_blurred = cv2.GaussianBlur(alpha_float, (feather_radius*2+1, feather_radius*2+1), 0)
        alpha = (alpha_blurred * 255).astype(np.uint8)
    
    # 应用alpha通道
    img[:, :, 3] = alpha
    
    # 额外处理：去除背景黑色像素（但保留主体边缘）
    # 对于alpha较小（透明/半透明）的像素，如果是黑色，强制完全透明
    transparent_mask = alpha < 30
    is_black = (img[:,:,0] < bg_threshold) & (img[:,:,1] < bg_threshold) & (img[:,:,2] < bg_threshold)
    final_transparent = transparent_mask & is_black
    img[final_transparent, 3] = 0
    
    # 保存
    cv2.imwrite(output_path, img)
    return True

def remove_black_background_advanced(image_path, output_path, method='edge_grow'):
    """
    高级去除黑边，提供多种策略
    
    方法:
        - 'edge_grow': 边缘生长法（推荐，效果好）
        - 'gradient': 梯度检测法（适合复杂边缘）
        - 'color_range': 颜色范围法（适合主体颜色单一）
    """
    img = cv2.imread(image_path, cv2.IMREAD_UNCHANGED)
    if img is None:
        return False
    
    # 确保RGBA格式
    if len(img.shape) == 3 and img.shape[2] == 3:
        img = cv2.cvtColor(img, cv2.COLOR_BGR2BGRA)
    
    h, w = img.shape[:2]
    
    if method == 'edge_grow':
        # 边缘生长法：从边缘向内部生长，找到真正的边界
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        
        # 1. 找到所有非黑色区域
        non_black = (img[:,:,0] > 30) | (img[:,:,1] > 30) | (img[:,:,2] > 30)
        non_black = non_black.astype(np.uint8) * 255
        
        # 2. 形态学膨胀，填充边缘缝隙
        kernel = np.ones((5,5), np.uint8)
        non_black_dilated = cv2.dilate(non_black, kernel, iterations=2)
        
        # 3. 找到连通区域
        num_labels, labels = cv2.connectedComponents(non_black_dilated)
        
        # 4. 找出最大的连通区域（主体）
        if num_labels > 1:
            label_counts = np.bincount(labels.flatten())
            label_counts[0] = 0  # 忽略背景
            main_label = np.argmax(label_counts)
            main_mask = (labels == main_label).astype(np.uint8) * 255
        else:
            main_mask = non_black_dilated
        
        # 5. 腐蚀一点点，去除可能的边缘噪点
        main_mask = cv2.erode(main_mask, kernel, iterations=1)
        
        # 6. 高斯模糊实现羽化
        alpha = cv2.GaussianBlur(main_mask, (7, 7), 0)
        img[:, :, 3] = alpha
        
    elif method == 'gradient':
        # 梯度检测法：通过图像梯度找到边缘
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        
        # 计算梯度
        grad_x = cv2.Sobel(gray, cv2.CV_64F, 1, 0, ksize=3)
        grad_y = cv2.Sobel(gray, cv2.CV_64F, 0, 1, ksize=3)
        gradient = np.sqrt(grad_x**2 + grad_y**2)
        gradient = np.uint8(np.clip(gradient, 0, 255))
        
        # 梯度阈值分割
        _, edge_mask = cv2.threshold(gradient, 30, 255, cv2.THRESH_BINARY)
        
        # 膨胀边缘
        kernel = np.ones((3,3), np.uint8)
        edge_mask = cv2.dilate(edge_mask, kernel, iterations=3)
        
        # 填充内部
        contours, _ = cv2.findContours(edge_mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        fill_mask = np.zeros_like(gray)
        cv2.drawContours(fill_mask, contours, -1, 255, -1)
        
        # 羽化
        alpha = cv2.GaussianBlur(fill_mask, (5, 5), 0)
        img[:, :, 3] = alpha
        
    elif method == 'color_range':
        # 颜色范围法：针对特定主体颜色（例如主体不是黑色）
        # 自动检测主体颜色范围
        non_black_pixels = img[(img[:,:,0] > 30) | (img[:,:,1] > 30) | (img[:,:,2] > 30)]
        
        if len(non_black_pixels) > 0:
            # 计算主体颜色均值
            mean_color = np.mean(non_black_pixels[:, :3], axis=0)
            std_color = np.std(non_black_pixels[:, :3], axis=0)
            
            # 创建颜色掩码
            lower = np.maximum(mean_color - 2*std_color, 0)
            upper = np.minimum(mean_color + 2*std_color, 255)
            
            color_mask = cv2.inRange(img[:,:,:3], lower.astype(np.uint8), upper.astype(np.uint8))
            
            # 形态学清理
            kernel = np.ones((5,5), np.uint8)
            color_mask = cv2.morphologyEx(color_mask, cv2.MORPH_CLOSE, kernel)
            color_mask = cv2.morphologyEx(color_mask, cv2.MORPH_OPEN, kernel)
            
            # 羽化
            alpha = cv2.GaussianBlur(color_mask, (5, 5), 0)
            img[:, :, 3] = alpha
    
    cv2.imwrite(output_path, img)
    return True

def batch_process(input_dir, output_dir, method='clean', **kwargs):
    """批量处理"""
    Path(output_dir).mkdir(parents=True, exist_ok=True)
    
    image_files = list(Path(input_dir).glob("*.png")) + list(Path(input_dir).glob("*.PNG"))
    
    if not image_files:
        print(f"在 {input_dir} 中没有找到PNG图片")
        return
    
    print(f"找到 {len(image_files)} 个图片文件")
    print(f"使用方法: {method}")
    print("-" * 50)
    
    for i, img_file in enumerate(image_files, 1):
        output_file = Path(output_dir) / img_file.name
        
        try:
            if method == 'clean':
                remove_black_background_clean(str(img_file), str(output_file), **kwargs)
            else:
                remove_black_background_advanced(str(img_file), str(output_file), method)
            
            print(f"[{i}/{len(image_files)}] ✓ {img_file.name}")
        except Exception as e:
            print(f"[{i}/{len(image_files)}] ✗ {img_file.name} - {str(e)}")
    
    print("-" * 50)
    print(f"完成！输出: {output_dir}")

def main():
    parser = argparse.ArgumentParser(description="去除黑色背景 - 无黑边版")
    parser.add_argument("-i", "--input", required=True, help="输入图片或文件夹")
    parser.add_argument("-o", "--output", required=True, help="输出图片或文件夹")
    parser.add_argument("-m", "--method", choices=['clean', 'edge_grow', 'gradient', 'color_range'],
                       default='clean', help="处理方法（默认clean）")
    parser.add_argument("--no-smooth", action="store_true", help="禁用边缘羽化")
    parser.add_argument("--feather", type=int, default=2, help="羽化半径（默认2）")
    parser.add_argument("--threshold", type=int, default=25, help="背景阈值（默认25）")
    
    args = parser.parse_args()
    
    kwargs = {
        'edge_smooth': not args.no_smooth,
        'feather_radius': args.feather,
        'bg_threshold': args.threshold
    }
    
    if os.path.isfile(args.input):
        remove_black_background_clean(args.input, args.output, **kwargs)
        print(f"完成: {args.output}")
    elif os.path.isdir(args.input):
        batch_process(args.input, args.output, args.method, **kwargs)
    else:
        print(f"错误：找不到 {args.input}")

if __name__ == "__main__":
    main()