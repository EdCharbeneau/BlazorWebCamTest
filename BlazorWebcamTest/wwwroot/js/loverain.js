//import { Engine, Render, Runner, World, Bodies } from '/js/matter.js';
const { Engine, Render, Runner, World, Bodies } = Matter;
const world = document.querySelector(".hearts");
const engine = Engine.create();
const runner = Runner.create();
const render = Render.create({
	canvas: world,
	engine: engine,
	options: {
		width: 1280,
		height: 720,
		background: "transparent",
		wireframes: false
	}
});
const boundaryOptions = {
	isStatic: true,
	render: {
		fillStyle: "transparent",
		strokeStyle: "transparent"
	}
};




const ground = Bodies.rectangle(640, 720, 1300, 4, boundaryOptions);
const leftWall = Bodies.rectangle(0, 360, 4, 740, boundaryOptions);
const rightWall = Bodies.rectangle(1280, 360, 4, 800, boundaryOptions);

export async function MakeItRain() {
	createBall();
	for (let i = 0; i < 100; i++) {
		World.add(engine.world, [createBall('./img/heart.png')])
	}
}


function createBall() {

	const ball = Bodies.circle(Math.round(Math.random() * 1280), -30, 25, {
		angle: Math.PI * (Math.random() * 2 - 1),
		friction: 0.001,
		frictionAir: 0.01,
		restitution: 0.8,
		render: {
			sprite: {
				texture: "./img/heart.png"
			}
		}
	});

	setTimeout(() => {
		World.remove(engine.world, ball);
	}, 10000);

	return ball;
}



Render.run(render);
Runner.run(runner, engine);

World.add(engine.world, [ground, leftWall, rightWall]);


