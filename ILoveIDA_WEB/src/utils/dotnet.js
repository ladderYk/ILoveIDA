export const setOn = () => {
  if (chrome.webview) {
    const Device = chrome.webview.hostObjects.Device;
    Device.setOn().then();
  }
  return Promise.reject("错误");

};
export const setOff = () => {
  if (chrome.webview) {
    const Device = chrome.webview.hostObjects.Device;
    Device.setOff().then();
  }
  return Promise.reject("错误");

};

export const addDeviceType = (form) => {
  if (chrome.webview) {
    const DeviceType = chrome.webview.hostObjects.DeviceType;
    DeviceType.addDeviceType(JSON.stringify(form)).then();
  }
  return Promise.reject("错误");

};

export const editDeviceType = (form) => {
  if (chrome.webview) {
    const DeviceType = chrome.webview.hostObjects.DeviceType;
    DeviceType.editDeviceType(JSON.stringify(form)).then();
  }
  return Promise.reject("错误");

};

export const getProtList = () => {
  if (chrome.webview) {
    const DeviceType = chrome.webview.hostObjects.Prot;
    return DeviceType.getProtList().then(handleRet);
  }
  return Promise.reject("错误");
};


export const addDevice = (form) => {
  if (chrome.webview) {
    const Device = chrome.webview.hostObjects.Device;
    Device.addDevice(JSON.stringify(form)).then();
  }
  return Promise.reject("错误");

};

export const editDevice = (form) => {
  if (chrome.webview) {
    const Device = chrome.webview.hostObjects.Device;
    Device.editDevice(JSON.stringify(form)).then();
  }
  return Promise.reject("错误");
};

export const getDeviceList = () => {
  if (chrome.webview) {
    const Device = chrome.webview.hostObjects.Device;
    return Device.getDeviceList().then(handleRet);
  }
  return Promise.reject("错误");
};
export const getDeviceData = (name) => {
  if (chrome.webview) {
    const Device = chrome.webview.hostObjects.Device;
    return Device.getDeviceData(name).then(handleRet);
  }
  return Promise.reject("错误");
};

const handleRet = (strResponse) => {
  var response = JSON.parse(strResponse);
  if (!response) return;
  return response["data"] ? response.data : response.result;
};
