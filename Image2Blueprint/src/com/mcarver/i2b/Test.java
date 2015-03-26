package com.mcarver.i2b;

import java.awt.Color;
import java.awt.Image;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.FileOutputStream;
import java.util.BitSet;

import javax.imageio.ImageIO;

public class Test {

	public static void main(String[] args) {
		float[] hsb = new float[3];
		try {
			FileOutputStream fout= new FileOutputStream("C:/Users/Michael/Code/SpaceEngineers/Image2Blueprint/jonathan.txt", false);
			BufferedImage image = ImageIO.read(new File("C:/Users/Michael/Code/SpaceEngineers/Image2Blueprint/jonathan.png"));
			for(int x=0; x < image.getWidth();x++){
				for(int y=0; y < image.getHeight(); y++){
					int rgb = image.getRGB(x, y);
					if ((rgb & 0xFF000000) == 0)
						continue;
					
					Color.RGBtoHSB((rgb & 0xFF0000) >> 16, (rgb & 0x00FF00) >> 8, (rgb & 0x0000FF), hsb);
					fout.write(generatePixel(x,y,hsb[0],hsb[1],hsb[2]).getBytes());
				}
			}
			fout.flush();
			fout.close();

		} catch (Exception ex) {
			ex.printStackTrace();
		}

	}
	
	/**
	 * 
	 * @param x
	 * @param y
	 * @param h 0.0 to 1.0
	 * @param s -1 to 1
	 * @param v -1 to 1
	 * @return
	 */

	public static String generatePixel(int x, int y, float h, float s, float v) {
		
		//v = (v-32768)/65536.0F;
		s = s * 2 - 1F;
		v = v*2-1F;

		return "<MyObjectBuilder_CubeBlock xsi:type=\"MyObjectBuilder_CubeBlock\">\n"
				+ "<SubtypeName>LargeBlockArmorBlock</SubtypeName>\n"
				+ "<Min x=\"" + x + "\" y=\"" + y + "\" z=\"0\" />\n"
				+ "<BlockOrientation Forward=\"Forward\" Up=\"Up\" />\n"
				+ "<ColorMaskHSV x=\"" + h + "\" y=\"" + s + "\" z=\"" + v + "\" />\n"
				+ "<ShareMode>None</ShareMode>\n"
				+ "<DeformationRatio>0</DeformationRatio>\n"
				+ "</MyObjectBuilder_CubeBlock>\n";
	}
}
