using UnityEngine;
using System.Collections;

public enum kScanSymbolType
{
    ZBAR_NONE        =      0,  /**< no symbol decoded */
    ZBAR_PARTIAL     =      1,  /**< intermediate status */
    ZBAR_EAN2        =      2,  /**< GS1 2-digit add-on */
    ZBAR_EAN5        =      5,  /**< GS1 5-digit add-on */
    ZBAR_EAN8        =      8,  /**< EAN-8 */
    ZBAR_UPCE        =      9,  /**< UPC-E */
    ZBAR_ISBN10      =     10,  /**< ISBN-10 (from EAN-13). @since 0.4 */
    ZBAR_UPCA        =     12,  /**< UPC-A */
    ZBAR_EAN13       =     13,  /**< EAN-13 */
    ZBAR_ISBN13      =     14,  /**< ISBN-13 (from EAN-13). @since 0.4 */
    ZBAR_COMPOSITE   =     15,  /**< EAN/UPC composite */
    ZBAR_I25         =     25,  /**< Interleaved 2 of 5. @since 0.4 */
    ZBAR_DATABAR     =     34,  /**< GS1 DataBar (RSS). @since 0.11 */
    ZBAR_DATABAR_EXP =     35,  /**< GS1 DataBar Expanded. @since 0.11 */
    ZBAR_CODE39      =     39,  /**< Code 39. @since 0.4 */
    ZBAR_PDF417      =     57,  /**< PDF417. @since 0.6 */
    ZBAR_QRCODE      =     64,  /**< QR Code. @since 0.10 */
    ZBAR_CODE93      =     93,  /**< Code 93. @since 0.11 */
    ZBAR_CODE128     =    128,  /**< Code 128 */

    /** mask for base symbol type.
     * @deprecated in 0.11, remove this from existing code
     */
    ZBAR_SYMBOL      = 0x00ff,
    /** 2-digit add-on flag.
     * @deprecated in 0.11, a ::ZBAR_EAN2 component is used for
     * 2-digit GS1 add-ons
     */
    ZBAR_ADDON2      = 0x0200,
    /** 5-digit add-on flag.
     * @deprecated in 0.11, a ::ZBAR_EAN5 component is used for
     * 5-digit GS1 add-ons
     */
    ZBAR_ADDON5      = 0x0500,
    /** add-on flag mask.
     * @deprecated in 0.11, GS1 add-ons are represented using composite
     * symbols of type ::ZBAR_COMPOSITE; add-on components use ::ZBAR_EAN2
     * or ::ZBAR_EAN5
     */
    ZBAR_ADDON       = 0x0700,
}

public enum kScanConfigOptions {
	ZBAR_CFG_ENABLE = 0,        /**< enable symbology/feature */
    ZBAR_CFG_ADD_CHECK,         /**< enable check digit when optional */
    ZBAR_CFG_EMIT_CHECK,        /**< return check digit when present */
    ZBAR_CFG_ASCII,             /**< enable full ASCII character set */
    ZBAR_CFG_NUM,               /**< number of boolean decoder configs */

    ZBAR_CFG_MIN_LEN = 0x20,    /**< minimum data length for valid decode */
    ZBAR_CFG_MAX_LEN,           /**< maximum data length for valid decode */

    ZBAR_CFG_UNCERTAINTY = 0x40,/**< required video consistency frames */

    ZBAR_CFG_POSITION = 0x80,   /**< enable scanner to collect position data */

    ZBAR_CFG_X_DENSITY = 0x100, /**< image scanner vertical scan density */
    ZBAR_CFG_Y_DENSITY,
}

public enum kCameraDevice {
	ZBAR_CAMERA_DEVICE_REAR,
	ZBAR_CAMERA_DEVICE_FRONT
}

public enum kCameraFlashMode {
	ZBAR_CAMERA_FLASH_MODE_OFF = -1,
	ZBAR_CAMERA_FLASH_MODE_AUTO = 0,
	ZBAR_CAMERA_FLASH_MODE_ON = 1
}

public class UZBarReaderViewController {
	public kScanSymbolType symbolType;
	public kScanConfigOptions configOpt;
	public int configSymbolValue;
	
	public kCameraDevice cameraDevice;
	public kCameraFlashMode cameraFlashMode;
	public bool showsZBarControls;
	
	public string zBarInfos;

	// Use this for initialization
	public UZBarReaderViewController() {
		this.symbolType = kScanSymbolType.ZBAR_I25;
		this.configOpt = kScanConfigOptions.ZBAR_CFG_ENABLE;
		this.configSymbolValue = 0;
		this.cameraDevice = kCameraDevice.ZBAR_CAMERA_DEVICE_REAR;
		this.cameraFlashMode = kCameraFlashMode.ZBAR_CAMERA_FLASH_MODE_AUTO;
		this.showsZBarControls = true;
	}
	
	public string getZBarInfos() {
		string infos = (int)this.symbolType+","+(int)this.configOpt+","+this.configSymbolValue+","+(int)this.cameraDevice+","+(int)this.cameraFlashMode+","+(this.showsZBarControls?1:0); 
		return infos;
	}
}
