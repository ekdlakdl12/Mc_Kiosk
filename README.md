<h1 align="center">
  <img src="https://capsule-render.vercel.app/api?type=rect&color=f4a261&height=300&section=header&text=WPF%20Kiosk%20Project&fontSize=90&fontColor=ffffff&fontAlign=center" alt="Kiosk Header">
</h1>

<div align="center">

<p align="center">
    <img src="https://img.shields.io/badge/STATUS-COMPLETED-2a9d8f?style=for-the-badge"/> 
    <img src="https://img.shields.io/badge/TYPE-WPF%20DESKTOP%20APP-264653?style=for-the-badge"/>
    <img src="https://img.shields.io/badge/DURATION-PERSONAL%20ASSIGNMENT-e76f51?style=for-the-badge"/>
</p>

</div>


## 🚀 프로젝트 개요

| 구분 | 내용 |
| :--- | :--- |
| **프로젝트명** | WPF Kiosk Program |
| **개발 목적** | JSON 데이터를 불러와 실제 키오스크와 유사한 주문 흐름을 구현 |
| **개발 방식** | 개인 과제 (구조 및 패턴 자유) |
| **필수 조건** | JSON 2개 이상 사용, 장바구니, 총 금액 계산, 주문 처리 |

---

## 🛠️ 기술 스택 (Tech Stack)

### 🖥️ Application
<img src="https://img.shields.io/badge/WPF-0078D4?style=for-the-badge&logo=windows&logoColor=white"/>
<img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white"/>

### 📄 데이터 처리
<img src="https://img.shields.io/badge/JSON-000000?style=for-the-badge&logo=json&logoColor=white"/>
<img src="https://img.shields.io/badge/Newtonsoft.Json-363636?style=for-the-badge"/>

### 🔧 기타
<img src="https://img.shields.io/badge/MVVM%20(Optional)-6a4c93?style=for-the-badge"/>
<img src="https://img.shields.io/badge/Git-181717?style=for-the-badge&logo=github&logoColor=white"/>

---

## 📁 프로젝트 구조
📦 Project  
├─ MainWindow.xaml  
├─ MainWindow.xaml.cs  
├─ Models  
│ └─ Menu.cs  
├─ ViewModels  
├─ Views  
├─ Services  
└─ Resources / Json  
├─ mc-burgers.json  (선택)  
├─ mc-cafe.json  
├─ mc-lunch.json  (선택)  
├─ mc-morning.json  
├─ mc-sides.json  
├─ mc-happy-meal.json  
└─ mc-happy-snack.json  


> 기본 조건은 **Menu.cs 포함**, 나머지 구조는 자유롭게 확장 가능.

---

## 📄 사용된 JSON 데이터

프로그램은 최소 **2개 이상의 JSON 파일**을 사용해 메뉴를 불러옵니다.

- 🍔 mc-burgers.json  
- ☕ mc-cafe.json  
- 🍱 mc-lunch.json  
- 🌅 mc-morning.json  
- 🍟 mc-sides.json  
- 🎁 mc-happy-meal.json  
- 🍪 mc-happy-snack.json  

각 JSON 데이터 예시는 아래와 같습니다.

```json
{
  "name": "빅맥",
  "price": 5200,
  "image": "img/bigmac.png"
}


---

