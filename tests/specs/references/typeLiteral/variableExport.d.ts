declare const supportsColor: {
	stdout: ColorInfo;
	stderr: ColorInfo;
};

type ColorInfo = string;

export default supportsColor;
